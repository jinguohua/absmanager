using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABS.Core.Models;
using SAFS.Core.Data;
using SAFS.Core.Permissions.Identity.Models;

namespace ABS.Core.Services
{

    public class MenuServices : SAFS.Core.Permissions.Service.PermissionService
    {
        public MenuServices(IUnitOfWork unitofwork) : base(unitofwork)
        {
        }

        public List<Menu> GetChildrenByMenuId(string menuId)
        {
            return MenuRepository.Entities.Where(o => o.ParentID == menuId)
                  .OrderBy(o => o.Sequence).ToList();
        }


        public void AddMenu(MenuViewModel vmenu)
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

            AddMenu(menu);

            //var model = AutoMapper.Mapper.Map<MenuViewModel, Menu>(vmenu);
            //model.Id = Guid.NewGuid().ToString();
            //AddMenu(model);
        }

        public void EditMenu(MenuViewModel vmenu)
        {
            var menu = Menus.Where(o => o.Id == vmenu.Id).SingleOrDefault();
            if (menu == null)
            {
                throw new Exception("菜单信息错误");
            }
            menu = AutoMapper.Mapper.Map<MenuViewModel, Menu>(vmenu);

            UpdateMenu(menu);
        }

    }
}
