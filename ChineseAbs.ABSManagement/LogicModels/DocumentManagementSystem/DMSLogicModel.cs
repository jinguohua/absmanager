using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class DMSLogicModel : BaseLogicModel
    {
        public DMSLogicModel(string userName, ProjectLogicModel projectLogicModel,
            DMSType dmsType = DMSType.PublishCooperationPlatform, string uid = "")//默认为DMS绑定Project，使用ProjectId作为Uid
            : base(userName, projectLogicModel)
        {
            m_dmsType = dmsType;
            m_uid = uid;

            LoadDMS();
        }

        private void LoadDMS()
        {
            List<DMS> dmsList = null;
            if (m_dmsType == DMSType.PublishCooperationPlatform)
            {
                dmsList = m_dbAdapter.DMS.GetByProjectId(m_project.Instance.ProjectId);
                if (dmsList.Count == 0)
                {
                    var dms = m_dbAdapter.DMS.Create(m_project.Instance.ProjectId);
                    dmsList.Add(dms);
                }

                CommUtils.Assert(dmsList.Count < 2, "Find more than 1 DMS by project guid [{0}]", m_project.Instance.ProjectGuid);
            }
            else if (m_dmsType == DMSType.Task)
            {
                dmsList = m_dbAdapter.DMS.GetByUid(m_dmsType, m_uid);
                if (dmsList.Count == 0)
                {
                    m_dms = m_dbAdapter.DMS.Create(m_dmsType);
                    dmsList.Add(m_dms);

                    var dmsTask = new DMSTask();
                    dmsTask.ShortCode = m_uid;
                    dmsTask.DmsId = m_dms.Id;
                    dmsTask.DmsTaskGuid = Guid.NewGuid().ToString();
                    m_dbAdapter.DMSTask.New(dmsTask);
                }

                CommUtils.Assert(dmsList.Count < 2, "Find more than 1 DMS by short code [{0}]", m_uid);
            }
            else if (m_dmsType == DMSType.DurationManagementPlatform) {
                dmsList = m_dbAdapter.DMS.GetByUid(m_dmsType, m_uid);
                if (dmsList.Count == 0)
                {
                    m_dms = m_dbAdapter.DMS.Create(m_dmsType);
                    dmsList.Add(m_dms);

                    var dmsDuration = new DMSDurationManagementPlatform();
                    dmsDuration.ProjectId = int.Parse( m_uid);
                    dmsDuration.DmsId = m_dms.Id;
                    dmsDuration.DmsDurationManagementPlatformGuid = Guid.NewGuid().ToString();
                    m_dbAdapter.DMSDuration.New(dmsDuration);
                }

                CommUtils.Assert(dmsList.Count < 2, "Find more than 1 DMS by projectid  [{0}]", m_uid);
            }
            else
            {
                CommUtils.Assert(false, "Unsupported document management system type [{0}]", m_dmsType.ToString());
            }

            m_dms = dmsList.First();
        }

        private void LoadSubFolders(DMSFolderLogicModel parentFolderLogicModel, List<DMSFolder> allfolders, int folderDepth)
        {
            CommUtils.Assert(folderDepth < DMSLogicModel.MaxFolderDepth, "Load folder failed, folder depth > {0}", DMSLogicModel.MaxFolderDepth);

            var parentFolderId = parentFolderLogicModel.Instance.Id;
            var subFolders = allfolders.Where(x => x.ParentFolderId == parentFolderId);
            parentFolderLogicModel.SubFolders = new List<DMSFolderLogicModel>();
            foreach (var subFolder in subFolders)
            {
                var subFolderLogicModel = new DMSFolderLogicModel(m_project);
                subFolderLogicModel.Instance = subFolder;
                subFolderLogicModel.ParentFolder = parentFolderLogicModel;

                LoadSubFolders(subFolderLogicModel, allfolders, folderDepth + 1);
                parentFolderLogicModel.SubFolders.Add(subFolderLogicModel);
            }
        }

        private void LoadFolders(DMSFolderLogicModel rootLogicModel, List<DMSFolder> allfolders)
        {
            var rootCount = AllFolders.Count(x => x.DmsFolderType == DmsFolderType.Root);
            CommUtils.Assert(rootCount < 2, "根结点数大于2, DMSGuid=[{0}]", m_dms.Guid);

            //初始化根结点文件夹
            DMSFolder rootFolder = null;
            if (rootCount == 0)
            {
                rootFolder = new DMSFolder();
                rootFolder.DMSId = m_dms.Id;
                rootFolder.Name = "root";
                rootFolder.Description = "root";
                rootFolder.ParentFolderId = null;
                rootFolder.DmsFolderType = DmsFolderType.Root;
                var now = DateTime.Now;
                rootFolder.CreateUserName = UserName;
                rootFolder.CreateTime = now;
                rootFolder.LastModifyUserName = UserName;
                rootFolder.LastModifyTime = now;
                rootFolder = m_dbAdapter.DMSFolder.Create(rootFolder);
            }
            else
            {
                rootFolder = AllFolders.Single(x => x.DmsFolderType == DmsFolderType.Root);
            }

            rootLogicModel.Instance = rootFolder;
            LoadSubFolders(rootLogicModel, allfolders, 0);
        }

        public DMSFolderLogicModel FindFolder(string folderGuid, DMSFolderLogicModel dmsFolderLogicModel)
        {
            if (dmsFolderLogicModel.Instance.Guid == folderGuid)
            {
                return dmsFolderLogicModel;
            }

            foreach (var subFolderLoginModel in dmsFolderLogicModel.SubFolders)
            {
                var findFolder = FindFolder(folderGuid, subFolderLoginModel);
                if (findFolder != null)
                {
                    return findFolder;
                }
            }

            return null;
        }

        public DMSFolderLogicModel FindFolder(string folderGuid)
        {
            return FindFolder(folderGuid, Root);
        }

        public DMSFolderLogicModel Root
        {
            get
            {
                if (m_root == null)
                {
                    m_root = new DMSFolderLogicModel(m_project);
                    LoadFolders(m_root, AllFolders);
                }
                return m_root;
            }
        }

        private List<DMSFolder> m_allFolders;

        public List<DMSFolder> AllFolders
        {
            get
            {
                if (m_allFolders == null)
                {
                    m_allFolders = m_dbAdapter.DMSFolder.GetByDMSId(m_dms.Id);
                }

                return m_allFolders;
            }
        }

        private DMSFolderLogicModel m_root;

        public DMS Instance { get { return m_dms; } }

        private DMS m_dms;

        public static readonly int MaxFolderDepth = 10;

        private DMSType m_dmsType;

        private string m_uid;
    }

    public enum DMSType
    {
        /// <summary>
        /// 发行协作平台
        /// </summary>
        PublishCooperationPlatform = 0,
        /// <summary>
        /// 存续期管理平台
        /// </summary>
        DurationManagementPlatform = 1,
        /// <summary>
        /// 绑定到工作上
        /// </summary>
        Task = -1,
    }
}
