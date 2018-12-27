//using System;
//using System.Web.Mvc;
//using ChineseAbs.ABSManagement;
//using ChineseAbs.ABSManagement.Utils;
//using ChineseAbs.ABSManagement.Models;
//using System.Web.Routing;

//namespace ChineseAbs.ABSManagementSite.Filters
//{
//    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
//    //public class GlobalAccessAttribute : ActionFilterAttribute
//    //{
//    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
//    //    {
//    //        var filterInfo = new FilterContextInfo(filterContext);
//    //        var userInfo = new UserInfo(filterInfo.Username);
//    //        var authority = m_dbAdapter.Authority;

//    //        foreach (var param in filterInfo.Parameters)
//    //        {
//    //            if (!string.IsNullOrEmpty(param.Key) && param.Value != null)
//    //            {
//    //                //Convert param name to enum, ignore case.
//    //                SecurityKey key;
//    //                if (Enum.TryParse(param.Key, true, out key))
//    //                {
//    //                    bool isAuthorized = false;
//    //                    string msg = "Check authority failed:" + param.Key + "=" + param.Value.ToString();
//    //                    try
//    //                    {
//    //                        isAuthorized = IsAuthorized(authority, userInfo, key, param.Value);
//    //                    }
//    //                    catch (Exception e)
//    //                    {
//    //                        msg += Environment.NewLine + "Message:" + e.Message;
//    //                        msg += Environment.NewLine + "StackTrace:" + e.StackTrace;
//    //                    }

//    //                    if (!isAuthorized)
//    //                    {
//    //                        var routeValue = new RouteValueDictionary(new
//    //                        {
//    //                            controller = "Error",
//    //                            action = "NoAccess",
//    //                            message = msg
//    //                        });
//    //                        filterContext.Result = new RedirectToRouteResult(routeValue);
//    //                        base.OnActionExecuting(filterContext);
//    //                        return;
//    //                    }
//    //                }
//    //            }
//    //        }

//    //        base.OnActionExecuting(filterContext);
//    //    }

//    //    private bool IsAuthorized(AuthorityManager authority, UserInfo userInfo, SecurityKey key, object value)
//    //    {
//    //        switch (key)
//    //        {
//    //            case SecurityKey.ProjectId:
//    //            {
//    //                int projectId = -1;
//    //                if (!int.TryParse(value.ToString(), out projectId))
//    //                {
//    //                    return false;
//    //                }

//    //                return authority.IsAuthorized(projectId);
//    //            }
//    //            case SecurityKey.ProjectGuid:
//    //            {
//    //                var guid = value.ToString();
//    //                var project = m_dbAdapter.Project.GetProjectByGuid(guid);
//    //                if (project.TypeId.HasValue
//    //                    && project.TypeId.Value == (int)ProjectSeriesStage.发行
//    //                    && project.ProjectSeriesId.HasValue)
//    //                {
//    //                    var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
//    //                    return m_dbAdapter.Permission.HasPermission(userInfo.UserName, projectSeries.Guid, ABSManagement.Models.PermissionType.Read);
//    //                }

//    //                return authority.IsAuthorized(project.ProjectId);
//    //            }
//    //            case SecurityKey.TaskId:
//    //            {
//    //                var taskId = int.Parse(value.ToString());
//    //                var task = m_dbAdapter.Task.GetTask(taskId);
//    //                var projectId = task.ProjectId;
//    //                return authority.IsAuthorized(projectId);
//    //            }
//    //            case SecurityKey.ShortCode:
//    //            {
//    //                var shortCode = value.ToString();
//    //                var task = m_dbAdapter.Task.GetTask(shortCode);
//    //                var projectId = task.ProjectId;
//    //                var project = m_dbAdapter.Project.GetProjectById(projectId);
//    //                if (project.TypeId.HasValue
//    //                    && project.TypeId.Value == (int)ProjectSeriesStage.发行
//    //                    && project.ProjectSeriesId.HasValue)
//    //                {
//    //                    var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
//    //                    return m_dbAdapter.Permission.HasPermission(userInfo.UserName, projectSeries.Guid, ABSManagement.Models.PermissionType.Read);
//    //                }

//    //                return authority.IsAuthorized(projectId);
//    //            }
//    //            case SecurityKey.DocumentId:
//    //            {
//    //                int documentId = -1;
//    //                if (!int.TryParse(value.ToString(), out documentId))
//    //                {
//    //                    return false;
//    //                }
//    //                string projectGuid = m_dbAdapter.Document.GetProjectGuidById(documentId);
//    //                var projectId = DbUtils.GetIdByGuid(projectGuid, "dbo.Project", "project_guid");
//    //                return authority.IsAuthorized(projectId);
//    //            }
//    //            default:
//    //                return true;
//    //        }
//    //    }

//    //    private DBAdapter m_dbAdapter { get { return new DBAdapter(); } }

//    //    enum SecurityKey
//    //    {
//    //        ProjectId,
//    //        ProjectGuid,
//    //        TaskId,
//    //        ShortCode,
//    //        DocumentId,
//    //    }
//    }
//}