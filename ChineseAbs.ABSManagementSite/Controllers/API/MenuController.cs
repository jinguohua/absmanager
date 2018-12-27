using System;
using System.Collections.Generic;
using System.Linq;
using ABS.Core.Services;
using SAFS.Core.Permissions.Identity.Models;
using AutoMapper;
using System.Web.Http;
using ChineseAbs.ABSManagement.Utils;
using ABS.Core.Models;

namespace ABS.ABSManagementSite.Controllers.API
{
    public class MenuController : BaseApiController
    {
        public MenuServices MenuService { get; set; }
        [HttpGet]
        public MenuViewModel List()
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

            return GetMenuViewData(rootMenu);

        }

        private MenuViewModel GetMenuViewData(Menu menu)
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
        [HttpPost]
        public void Add(MenuViewModel vmenu)
        {
            MenuService.AddMenu(vmenu);
        }
        [HttpPost]
        public void Edit(MenuViewModel vmenu)
        {
            MenuService.EditMenu(vmenu);
        }
        [HttpPost]
        public void Delete(string id)
        {
            MenuService.RemoveMenu(id);
        }
        [HttpGet]
        public List<string[]> Functions()
        {
            var funList = MenuService.Functions.ToList();
            var data = funList.Select(o => new { id = o.Id, text = o.Name }).ToArray();
            List<string[]> arr = new List<string[]>();
            foreach (var item in funList)
            {
                arr.Add(new[] { item.Id, item.Name });
            }
            return arr;
        }
    }
}
