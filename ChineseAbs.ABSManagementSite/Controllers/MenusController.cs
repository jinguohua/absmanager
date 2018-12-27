using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABS.ABSManagementSite.Models;
using AutoMapper;
using ChineseAbs.ABSManagementSite.Controllers;
using SAFS.Core.Permissions.Identity.Models;
using SAFS.Utility.Web;

namespace ABS.ABSManagementSite.Controllers
{
    public class MenusController : BaseController
    {
        // GET: Menus
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetMenusList()
        {
            var rootMenu = MenuService.Menus
                .FirstOrDefault(o => string.IsNullOrEmpty(o.ParentID));

            if (rootMenu == null)
            {
                rootMenu = new Menu()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "菜单目录",
                    Sequence = 0,
                    Description = "菜单目录",
                };

                MenuService.AddMenu(rootMenu);
            }
            //var childrenData = MenuService.GetChildrenByMenuId(rootMenu.Id);
            //rootMenu.Children = childrenData;
            //var data = Mapper.Map<Menu, MenuViewModel>(rootMenu);
            var data = GetMenuViewData(rootMenu);
            return Json(new JsonResultDataEntity<object>() { Code = 0, Data = data });
        }
        public MenuViewModel GetMenuViewData(Menu menu)
        {
            var view = Mapper.Map<Menu, MenuViewModel>(menu);
            view.Children = new List<MenuViewModel>();
            var children = MenuService.GetChildrenByMenuId(menu.Id);
            foreach (var item in children)
            {
                var viewChildren = GetMenuViewData(item);
                view.Children.Add(viewChildren);
            }

            return view;

        }
        public JsonResult AddMenu(MenuViewModel vmenu)
        {
            Menu menu = new Menu()
            {
                Id = Guid.NewGuid().ToString(),
                Name = vmenu.Name,
                FunctionID = vmenu.FunctionID,
                ParentID = vmenu.ParentID,
                Sequence = vmenu.Sequence,
                Description = vmenu.Description,
                CssClassName = vmenu.CssClassName,
                URL = vmenu.URL,
                Extension = vmenu.Extension,
            };
            MenuService.AddMenu(menu);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }
        public JsonResult EditMenu(MenuViewModel vmenu)
        {
            var menu = MenuService.Menus.Where(o => o.Id == vmenu.Id).SingleOrDefault();
            menu.Name = vmenu.Name;
            menu.FunctionID = vmenu.FunctionID;
            menu.ParentID = vmenu.ParentID;
            menu.Sequence = vmenu.Sequence;
            menu.Description = vmenu.Description;
            menu.CssClassName = vmenu.CssClassName;
            menu.URL = vmenu.URL;
            menu.Extension = vmenu.Extension;

            MenuService.UpdateMenu(menu);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }
        public JsonResult DeleteMenu(string id)
        {
            MenuService.RemoveMenu(id);

            return Json(new JsonResultDataEntity<object>() { Code = 0 });
        }
        [HttpPost]
        public JsonResult GetFunctions()
        {
            var funList = MenuService.Functions.ToList();
            var data = funList.Select(o =>new {id=o.Id, text=o.Name }).ToArray();
            List<string[]> arr = new List<string[]>();
            foreach (var item in funList)
            {
                arr.Add(new[] { item.Id, item.Name });
            }

            return Json(new JsonResultDataEntity<object>() { Code = 0, Data = arr });
        }
    }
}