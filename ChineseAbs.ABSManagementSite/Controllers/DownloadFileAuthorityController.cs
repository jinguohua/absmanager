using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class DownloadFileAuthorityController : BaseController
    {
        [HttpPost]
        public ActionResult GetDownloadFileAuthorities(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var authorities = projectLogicModel.Authority.DownloadFile.Authorities;

                var result = authorities.Select(x => new
                {
                    UserName = x.UserName,
                    DownloadAuthorityType = x.DownloadFileAuthorityType.ToString(),
                    LastModifyTime = DateUtils.IsNormalDate(x.LastModifyTime) ? Toolkit.DateTimeToString(x.LastModifyTime) : "-",
                });

                return ActionUtils.Success(result);
            });
        }

        //projectGuid为空时，对当前机构下所有产品设置权限
        [HttpPost]
        public ActionResult UpdateDownloadFileAuthorities(string projectGuid, string userNames, string downloadFileAuthorityType)
        {
            return ActionUtils.Json(() =>
            {
                List<string> projectGuids = new List<string>();
                if (string.IsNullOrWhiteSpace(projectGuid))
                {
                    var projectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                    var projects = m_dbAdapter.Project.GetProjects(projectIds);
                    projectGuids = projects.Select(x => x.ProjectGuid).ToList();
                }
                else
                {
                    projectGuids.Add(projectGuid);
                }

                int updateRecordCount = 0;
                foreach (var guid in projectGuids)
                {
                    var projectLogicModel = new ProjectLogicModel(CurrentUserName, guid);
                    var authorities = projectLogicModel.Authority.DownloadFile.Authorities;

                    var userNameList = CommUtils.Split(userNames).ToList();

                    foreach (var userName in userNameList)
                    {
                        CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(userName), "用户[{0}]不存在", userName);
                        CommUtils.Assert(authorities.Any(x => x.UserName.Equals(userName, System.StringComparison.CurrentCultureIgnoreCase)),
                            "找不到用户[{0}]", userName);
                    }

                    var authorityType = CommUtils.ParseEnum<DownloadFileAuthorityType>(downloadFileAuthorityType);

                    foreach (var userName in userNameList)
                    {
                        CommUtils.Assert(m_dbAdapter.Authority.IsAuthorized(projectLogicModel.Instance.ProjectId, userName),
                            "用户[{0}]没有查看产品[{1}]的权限，设置下载文件权限失败", userName, projectLogicModel.Instance.Name);

                        var authority = authorities.Single(x => x.UserName.Equals(userName, System.StringComparison.CurrentCultureIgnoreCase));
                        if (string.IsNullOrWhiteSpace(authority.CreateUserName))
                        {
                            m_dbAdapter.DownloadFileAuthority.Create(projectLogicModel.Instance.ProjectId, userName, authorityType);

                            NewEditDownloadFileAuthorityLog(projectLogicModel.Instance.ProjectId,
                                "创建下载文件权限，userName=[{0}];authorityType=[{1}];projectId=[{2}]",
                                userName, authorityType.ToString(), projectLogicModel.Instance.ProjectId);
                        }
                        else
                        {
                            authority.DownloadFileAuthorityType = authorityType;
                            m_dbAdapter.DownloadFileAuthority.Update(authority);

                            NewEditDownloadFileAuthorityLog(projectLogicModel.Instance.ProjectId,
                                "更新下载文件权限，userName=[{0}];authorityType=[{1}];projectId=[{2}]",
                                userName, authorityType.ToString(), projectLogicModel.Instance.ProjectId);
                        }

                        ++updateRecordCount;
                    }
                }

                return ActionUtils.Success(updateRecordCount);
            });
        }

        private void NewEditDownloadFileAuthorityLog(int? projectId, string description, params object[] args)
        {
            description = CommUtils.FormatString(description, args);
            m_dbAdapter.Project.NewEditProductLog(EditProductType.EditDownloadFileAuthority,
                projectId, description, "只有超级管理员才能够进入的权限管理页面");
        }
    }
}

