using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class DMSFolderLogicModel : BaseLogicModel
    {
        public DMSFolderLogicModel(ProjectLogicModel projectLogicModel)
            : base(projectLogicModel.UserName, projectLogicModel)
        {

        }

        public DMSFolderLogicModel(string username)
            : base(username, null)
        {

        }

        public DMSFolder Instance { get; set; }

        public DMSFolderLogicModel ParentFolder { get; set; }

        public List<DMSFolderLogicModel> SubFolders { get; set; }

        private bool _ignoreNull = true;
        public bool IgnoreNull { get { return _ignoreNull; } set { _ignoreNull = value; } }

        public List<DMSFileLogicModel> Files
        {
            get
            {
                if (m_files == null)
                {
                    var fileSeriesList = m_dbAdapter.DMSFileSeries.GetFileSeriesByFolderId(Instance.Id);
                    var fileSeriesIds = fileSeriesList.Select(x => x.Id);
                    var files = m_dbAdapter.DMSFile.GetFilesByFileSeriesIds(fileSeriesIds);

                    if (!IgnoreNull)
                    {
                        var seriesIds = files.GroupBy(x => x.DMSFileSeriesId).Select(x => x.Key);
                        var fileSeriesNofileList = fileSeriesList.Where(x => !seriesIds.Contains(x.Id)).ToList();
                        fileSeriesNofileList.ForEach(x =>
                        {
                            var file = new DMSFile();
                            file.DMSId = x.DMSId;
                            file.DMSFileSeriesId = x.Id;
                            files.Add(file);
                        });
                    }

                    var dict = files.GroupBy(x => x.DMSFileSeriesId).ToDictionary(x => x.Key);

                    m_files = new List<DMSFileLogicModel>();
                    foreach (var key in dict.Keys)
                    {
                        var allFiles = dict[key].OrderByDescending(x => x.LastModifyTime).ToList();

                        var fileLogicModel = new DMSFileLogicModel(m_project);
                        fileLogicModel.AllVerFiles = allFiles;
                        fileLogicModel.FileSeries = fileSeriesList.Single(x => x.Id == key);
                        m_files.Add(fileLogicModel);
                    }
                }
                return m_files;
            }
        }
 
        public string GetParentFolderPath()
        {
            var path = string.Empty;
            var folder = this;
            while (folder.ParentFolder != null)
            {
                folder = folder.ParentFolder;
                path = folder.Instance.Name + FileUtils.PathSeparator + path;
            }

            return path;
        }

        private List<DMSFileLogicModel> m_files;

        public dynamic ToTree()
        {
            return new
            {
                title = Instance.Name,
                key = Instance.Guid,
                description = Instance.Description,
                createUserName = Instance.CreateUserName,
                createTime = Instance.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                children = SubFolders.Select(x => x.ToTree())
            };
        }
    }
}
