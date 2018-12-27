using ABS.AssetManagement.Models;
using ABS.Infrastructure.Models;
using SAFS.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class AppUserConfig : EntityConfigurationBase<AppUser, string>
    {
        public override void AppendConfig()
        {
            this.HasOptional(o => o.Activity).WithRequired(o => o.User);
            this.HasMany(o => o.Organizations).WithMany(o => o.Members).Map(
                m => m.ToTable("OrganizationUser"));
        }
    }

    public class CodeCategoryConfig : EntityConfigurationBase<CodeCategory, int>
    {
    }

    public class ProjectConfig: EntityConfigurationBase<Project, int>
    {

    }

    public class AssetDataConfig: EntityConfigurationBase<AssetData, int>
    {
        public override void AppendConfig()
        {
            this.HasOptional(o => o.Rate).WithRequired(o => o.AssetData);
        }
    }

    public class RealDateRuleConfig : EntityConfigurationBase<DateRule, long>
    {
        public override void AppendConfig()
        {
            
        }
    }

    public class AssetPackageConfig : EntityConfigurationBase<AssetPackage, int>
    {

    }
}
