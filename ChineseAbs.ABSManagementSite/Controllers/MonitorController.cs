using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using ChineseAbs.Logic;
using System;
using System.IO;
using System.Web.Mvc;
using System.Linq;


namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class MonitorController : BaseController
    {
        public DealService ds;

        // GET: /Monitor
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Monitor/project/:id
        //[HttpGet]
        //public ActionResult Project(string projectGuid, int? page, int? status, int? pageSize, DateTime? start, DateTime? end)
        //{
        //    if (projectGuid == null)
        //    {
        //        return RedirectToAction("Index", "Monitor");
        //    };
        //    MonitorViewModel monitorview = new MonitorViewModel();
        //    //DealService ds = new DealService();
        //    var proj = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
        //    if (proj.CnabsDealId.HasValue)
        //    {
        //        monitorview.Projectinfo = MonitorConvertion.ConvertProjectInfo(ds.GetDealData(proj.CnabsDealId.Value), projectGuid);
        //    }
        //    else
        //    {
        //        var rootFolder = WebConfigUtils.RootFolder;
        //        proj.Model = m_dbAdapter.Project.GetModel(proj.ModelId);

        //        var modelFolder = Path.Combine(rootFolder, proj.Model.ModelFolder);
        //        var ymlFilePath = modelFolder + @"\Script.yml";
        //        if (System.IO.File.Exists(ymlFilePath))
        //        {
        //            using (StreamReader sr = new StreamReader(ymlFilePath))
        //            {
        //                var nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
        //                if (nancyDealData != null)
        //                {
        //                    monitorview.Projectinfo.firstPaymentDate = null;
        //                    monitorview.Projectinfo = new ProjectInfo()
        //                    {
        //                        guid = proj.ProjectGuid,
        //                        fullName = proj.Name,
        //                        closingDate = nancyDealData.ScheduleData.ClosingDate,
        //                        legalMaturityDate = nancyDealData.ScheduleData.LegalMaturity,
        //                        firstPaymentDate = nancyDealData.ScheduleData.PaymentSchedule.Periods.Count > 0 ?
        //                            nancyDealData.ScheduleData.PaymentSchedule.Periods[0].PaymentDate : monitorview.Projectinfo.firstPaymentDate,
        //                        paymentFrequency = Toolkit.PaymentFrequency(nancyDealData.ScheduleData.PaymentPerYear)
        //                        //TODO: 监管机构 产品类型
        //                        //regulator
        //                        //productType
        //                    };
        //                }
        //            }
        //        }
        //    }
        //    monitorview.NewsDetail = m_dbAdapter.News.GetProjectNewsDetail(proj.ProjectId, page, status, pageSize, start, end);
        //    monitorview.Agencys = MonitorConvertion.ConvertAgencies(
        //        m_dbAdapter.Contact.GetContactsByProjectId(proj.ProjectId));

        //    monitorview.Projects = MonitorConvertion.ConvertProjectList(
        //        m_dbAdapter.Project.GetProjects(m_dbAdapter.Authority.GetAuthorizedProjectIds()));

        //    return View(monitorview);
        //}

        [HttpPost]
        public ActionResult GetProjectInfoAndContacts(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                //权限检查
                var authorizedIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(authorizedIds.Contains(project.ProjectId), "当前用户没有读取产品[{0}]的权限", project.Name);

                var projectInfo = new ProjectInfo();
                if (project.CnabsDealId.HasValue)
                {
                    DealService ds = new DealService();
                    projectInfo = MonitorConvertion.ConvertProjectInfo(ds.GetDealData(project.CnabsDealId.Value), projectGuid);
                }
                else
                {
                    var rootFolder = WebConfigUtils.RootFolder;
                    project.Model = m_dbAdapter.Project.GetModel(project.ModelId);

                    var modelFolder = Path.Combine(rootFolder, project.Model.ModelFolder);
                    var ymlFilePath = modelFolder + @"\Script.yml";
                    if (System.IO.File.Exists(ymlFilePath))
                    {
                        using (StreamReader sr = new StreamReader(ymlFilePath))
                        {
                            var nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                            if (nancyDealData != null)
                            {
                                DateTime? date = null;
                                projectInfo = new ProjectInfo()
                                {
                                    guid = project.ProjectGuid,
                                    fullName = project.Name,
                                    closingDate = nancyDealData.ScheduleData.ClosingDate,
                                    legalMaturityDate = nancyDealData.ScheduleData.LegalMaturity,
                                    firstPaymentDate = nancyDealData.ScheduleData.PaymentSchedule.Periods.Count > 0 ?
                                        nancyDealData.ScheduleData.PaymentSchedule.Periods[0].PaymentDate : date,
                                    paymentFrequency = Toolkit.PaymentFrequency(nancyDealData.ScheduleData.PaymentPerYear),
                                    //TODO: 监管机构 产品类型
                                    //regulator
                                    //productType
                                };
                            }
                        }
                    }

                }

                var result = new
                {
                    ProjectInfo = new
                    {
                        Guid = projectInfo.guid,
                        FullName = projectInfo.fullName,
                        ClosingDate = Toolkit.DateToString(projectInfo.closingDate),
                        LegalMaturityDate = Toolkit.DateToString(projectInfo.legalMaturityDate),
                        FirstPaymentDate = Toolkit.DateToString(projectInfo.firstPaymentDate),
                        PaymentFrequency = projectInfo.paymentFrequency,
                        Regulator = Toolkit.ToString(projectInfo.regulator),
                        ProductType = Toolkit.ToString(projectInfo.productType),
                    },
                    Contacts = m_dbAdapter.Contact.GetContactsByProjectId(project.ProjectId),
                };
                
                return ActionUtils.Success(result);
            });
        }

         [HttpPost]
        public ActionResult GetNewsDetail(string projectGuid, int? page, int? status, int? pageSize, DateTime? start, DateTime? end)
        {
            return ActionUtils.Json(() =>
            {
                //权限检查
                var authorizedIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(authorizedIds.Contains(project.ProjectId), "当前用户没有读取产品[{0}]的权限", project.Name);

                var newsDetail = m_dbAdapter.News.GetProjectNewsDetail(project.ProjectId, page, status, pageSize, start, end);
                var result = new
                {
                    News = newsDetail.News.ConvertAll(x => new
                    {
                        ID = x.ID.ToString(),
                        URL = x.URL,
                        ProjectID = x.ProjectID,
                        Title = Toolkit.LongTitleDisplay(x.Title, 32),
                        Source = x.Source,
                        Status = x.Status,
                        OriginDate = ChineseAbs.ABSManagement.Utils.DateUtils.IsNormalDate(x.OriginDate) ? Toolkit.DateToString(x.OriginDate) : "-"
                    }),
                    StatisticInfo = new
                    {
                        Min = Toolkit.DateToString(newsDetail.Min),
                        Max = Toolkit.DateToString(newsDetail.Max),
                        TotalCount = newsDetail.TotalCount,
                        ReadCount = newsDetail.ReadCount,
                        UnreadCount = newsDetail.UnreadCount,
                        TotalPage = newsDetail.TotalPage
                    }
                };
                return ActionUtils.Success(result);
            });
        }

        // POST: /Monitor/addnews
        [HttpPost]
        public ActionResult AddNews(string projectGuid, News n)
        {
            if (projectGuid == null)
            {
                return Json(false);
            }
            var proj = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            n.ProjectID = proj.ProjectId;
            var nid = m_dbAdapter.News.CreateNewsRecord(n);
            if (nid != 0)
            {
                return Json(true);
            }
            return Json(false);
        }

        //POST: /Monitor/setreadajax/:id
        [HttpPost]
        public ActionResult SetRead(string projectGuid, Int64 nid)
        {
            return ActionUtils.Json(() =>
            {
                var authorizedIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(authorizedIds.Contains(project.ProjectId), "当前用户没有修改产品[{0}]的权限", project.Name);

                var result = true;
                if (nid == 0)
                {
                    //set all read
                    if (!m_dbAdapter.News.SetAllNewsRead(project.ProjectId))
                    {
                        result = false;

                    }
                    else
                    {
                        return ActionUtils.Success(result);
                    }
                }
                //set single read
                if (!m_dbAdapter.News.SetNewsStatusRead(nid, project.ProjectId))
                {
                    result = false;
                }
            return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult SubscribeNews(string projectGuids)
        {
            return ActionUtils.Json(() =>
            {
                var projectGuidList = CommUtils.Split(projectGuids).ToList();
                CommUtils.Assert(m_dbAdapter.NegativeNews.CheckProjectsPermission(projectGuidList), "当前用户没有操作该产品权限,请刷新重试");

                projectGuidList.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x))
                    {
                        var projectId = m_dbAdapter.Project.GetProjectByGuid(x).ProjectId;
                        m_dbAdapter.NegativeNews.New(m_dbAdapter.Project.GetProjectByGuid(x).ProjectId, CurrentUserName);
                    }
                });

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult CancelSubscribe(string projectGuids)
        {
            return ActionUtils.Json(() =>
            {
                var projectGuidList = CommUtils.Split(projectGuids).ToList();
                CommUtils.Assert(m_dbAdapter.NegativeNews.CheckProjectsPermission(projectGuidList), "当前用户没有操作该产品权限,请刷新重试");

                projectGuidList.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x))
                    {
                        var projectId = m_dbAdapter.Project.GetProjectByGuid(x).ProjectId;
                        var negativeNews = m_dbAdapter.NegativeNews.GetByProjIdUser(projectId, CurrentUserName);
                        if (negativeNews != null)
                        {
                            m_dbAdapter.Permission.HasPermission(CurrentUserName, x, PermissionType.Execute);
                            m_dbAdapter.NegativeNews.Delete(negativeNews);
                        }
                    }
                });
                return ActionUtils.Success(1);

            });
        }

        [HttpPost]
        public ActionResult GetSubscribeStatus(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.NegativeNews.CheckProjectPermission(projectGuid), "当前用户没有操作该产品权限,请刷新重试");
                var projectId = m_dbAdapter.Project.GetProjectByGuid(projectGuid).ProjectId;
                var hasNegativeNews = m_dbAdapter.NegativeNews.HasNegativeNews(projectId, CurrentUserName);
                return ActionUtils.Success(new { isExist = hasNegativeNews });
            });
        }

    }
}
