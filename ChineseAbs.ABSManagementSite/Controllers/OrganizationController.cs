using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABS.Core.Models;
using ABS.Core.Services;
using AutoMapper;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Controllers;
using SAFS.Utility.Web;

namespace ABS.ABSManagementSite.Controllers
{
    public class OrganizationController : BaseController
    {
        public OrganizationsService OrganizationService { get; set; }

        // GET: Organization
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetOrganizationList()
        {
            var data = OrganizationService.Organizations.ToList();
            var view = data.Select(o => new
            {
                id = o.Id,
                parent = o.ParentID == null ? "#" : o.ParentID.ToString(),
                text = o.Name,
            }).ToList();

            return Json(view, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddOrganization(OrganizationViewModel vmodel)
        {

            OrganizationService.Add(vmodel);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }


        public JsonResult EditOrganization(OrganizationViewModel vmodel)
        {
            OrganizationService.Edit(vmodel);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }


        public JsonResult DeleteOrganization(int id)
        {
            CommUtils.Assert(id != 0, "数据错误,不能删除");
            OrganizationService.Delete(id);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }

        public JsonResult AddMemberForOrganiations(int organizationId, string[] userIds)
        {
            var users = UserService.GetUsersByIds(userIds);

            OrganizationService.AddMemberForOrganiations(organizationId, users);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }


        public JsonResult RemoveMemberForOrganiations(int organizationId, string userId)
        {
            var user = UserService.GetUserById(userId);
            CommUtils.Assert(userId != null, "数据错误,删除失败");
            OrganizationService.RemoeMemberForOrganiations(organizationId, user);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }

        public JsonResult GetMemberList(int organizationId)
        {
            if (organizationId == 0)
            {
                return Json(new JsonResultDataEntity<object>() { Code = 0 });
            }

            var memberList = OrganizationService.GetMemberByOrganiationId(organizationId);
            var members = memberList.Where(o => !o.IsDeleted).ToList().Select(x => new
            {
                id = x.Id,
                UserName = x.UserName,
                NickName = string.IsNullOrEmpty(x.NickName) ? x.UserName : x.NickName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                RoleNames = string.Join(",", x.Roles.Select(o => o.Description).ToList()),
            }).ToList();

            var allUsers = UserService.GetAllUser();
            var users = allUsers.Except(memberList).Select(o => new
            {
                id = o.Id,
                text = o.Name
            }).ToList();

            return Json(new JsonResultDataEntity<object>() { Code = 0, Data = new { members, users, } });
        }
    }
}