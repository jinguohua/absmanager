using Microsoft.AspNet.Identity;
using SAFS.Core.Data.Entity.Migrations;
using SAFS.Core.Permissions.Identity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class DbMigrations : ISeedAction
    {
        public int Order => 0;

        public void Action(DbContext context)
        {
            var adminRole = context.Set<Role>().FirstOrDefault(o => o.Name.Equals("Admin"));
            if (adminRole == null)
            {
                adminRole = new Role() { Name = "Admin", Description = "Administrator", CreatedTime = DateTime.Now, Creator = "System" };
                context.Set<Role>().Add(adminRole);
            }
            var adminUser = context.Set<AppUser>().FirstOrDefault(o => o.UserName == "Admin");
            if (adminUser == null)
            {
                adminUser = new AppUser()
                {
                    UserName = "admin",
                    NickName = "admin",
                    PasswordHash = new PasswordHasher().HashPassword("ABS123456"),
                    CreatedTime = DateTime.Now,
                    Creator = "System",
                    IsActive = true,
                    IsDeleted = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                adminUser.Roles.Add(adminRole);
                context.Set<User>().Add(adminUser);
            }

        }
    }
}
