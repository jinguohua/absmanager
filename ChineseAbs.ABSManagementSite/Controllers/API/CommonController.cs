using ABS.Core;
using ABS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ABS.ABSManagementSite.Controllers.API
{
    public class CommonController : BaseApiController
    {
        public BaseCodeService BaseCodeService { get; set; }
        public OrganizationsService OrganizationsService { get; set; }
        

        public class SelectItem
        {
            public string key { get; set; }

            public string value { get; set; }
        }
        [HttpGet]
        public List<SelectItem> DataItems(string category, string search)
        {
            search = search ?? "";
            bool notLike = search.StartsWith("=");
            search = search.TrimStart('=');
            List<SelectItem> result = new List<SelectItem>();
            switch (category.ToLower())
            {
                case "company":
                    result = GetCompanyData(search, notLike);
                    break;
                case "user":
                    result = GetUserData(search, notLike);
                    break;
                case "organization":
                    result = GetOrganizationData(search, notLike);
                    break;
                case "role":
                    result = GetRoleData(search, notLike);
                    break;
                default:
                    if (category.StartsWith("enum.", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var enumtype = category.Substring("enum.".Length);
                        result = GetEnums(enumtype, search, notLike);
                    }
                    else
                    {
                        result = GetCategory(category, search, notLike);
                    }
                    break;
            }
            return result;
        }

        private List<SelectItem> GetCompanyData(string search, bool notLike)
        {
            var viewModels = CompanyService.Companies;

            if (!String.IsNullOrEmpty(search))
            {
                if (notLike)
                {
                    string[] values = search.Split(',');
                    viewModels = viewModels.Where(o => values.Contains(o.Id.ToString()) || values.Contains(o.ShortName)).ToList();
                }
                else
                {
                    viewModels = viewModels.Where(o =>
                        o.Name.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1
                        ||
                        o.ShortName.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1
                    ).ToList();
                }
            }
            else if (notLike)
            {
                return new List<SelectItem>();
            }

            return  viewModels.Select(c => new SelectItem()
            {
                key = c.Id.ToString(),
                value = c.ShortName
            }).ToList();
        }

        private List<SelectItem> GetUserData(string search, bool notLike)
        {
            var query = UserService.UserRepository.NoTrackingEntities.Where(m => m.IsDeleted == false);
            if (!String.IsNullOrEmpty(search))
            {
                if (notLike)
                {
                    string[] values = search.Split(',');
                    query = query.Where(o => values.Contains(o.Id) || values.Contains(o.UserName));
                }
                else
                {
                    query = query.Where(o => o.NickName.Contains(search) || o.UserName.Contains(search));
                }
            }
            else if (notLike)
            {
                return new List<SelectItem>();
            }

            return query.OrderBy(o => o.NickName)
                .Select(o => new { o.Id, o.NickName, o.UserName })
                .ToList()
                .Select(o => new SelectItem() { key = o.Id, value = String.IsNullOrEmpty(o.NickName) ? o.UserName : o.NickName })
                .ToList();

        }

        private List<SelectItem> GetCategory(string categoryCode, string search, bool notLike)
        {
            List<SelectItem> result = new List<SelectItem>();
            var query = BaseCodeService.GetItemsByCategory(categoryCode);
            if (!string.IsNullOrEmpty(search))
            {
                if (notLike)
                {
                    string[] values = search.Split(',');
                    query = query.Where(o => values.Contains(o.Key)).ToList();
                }
                else
                {
                    query = query.Where(o => o.Value.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1 || o.Key.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1).ToList();
                }
            }
            else if (notLike)
            {
                return result;
            }
            result = query.OrderBy(o => o.Value)
                .Select(o => new SelectItem() { key = o.Key, value = o.Value })
                .ToList();

            return result;
        }

        private List<SelectItem> GetOrganizationData(string search, bool notLike)
        {
            List<SelectItem> result = new List<SelectItem>();
            var query = OrganizationsService.OrganizationRespository.NoTrackingEntities;
            if (!String.IsNullOrEmpty(search))
            {
                if (notLike)
                {
                    string[] values = search.Split(',');
                    query = query.Where(o => values.Contains(o.Id.ToString()) || values.Contains(o.Name));
                }
                else
                {
                    query = query.Where(o => o.IdentityKey.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1
                                            || o.Name.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1);
                }
            }
            else if (notLike)
            {
                return result;
            }
            result = query.OrderBy(o => o.Name)
                //.Take(length)
                .Select(o => new { o.Id, o.Name })
                .ToList()
                .Select(o => new SelectItem() { key = o.Id.ToString(), value = o.Name })
                .ToList();
            return result;
        }

        private List<SelectItem> GetRoleData(string search, bool notLike)
        {
            List<SelectItem> result = new List<SelectItem>();

            var query = UserService.GetAllRoles();
            if (!string.IsNullOrEmpty(search))
            {
                if (notLike)
                {
                    string[] values = search.Split(',');
                    query = query.Where(o => values.Contains(o.Id) || values.Contains(o.Name)).ToList();
                }
                else
                {
                    query = query.Where(o => o.Description.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1
                                            || o.Name.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1).ToList();
                }
            }
            else if (notLike)
            {
                return result;
            }
            result = query.OrderBy(o => o.Name)
                .Select(o => new { o.Id, Name = o.Description ?? o.Name })
                .Select(o => new SelectItem() { key = o.Id.ToString(), value = o.Name })
                .ToList();
            return result;
        }

        private List<SelectItem> GetEnums(string enumType, string search, bool notLike)
        {
            List<SelectItem> result = new List<SelectItem>();
            var enumItems = EnumHelper.Enums.Where(o => o.Key.FullName.EndsWith(enumType, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (enumItems.Value == null || enumItems.Value.Count == 0)
                return new List<SelectItem>();

            var query = enumItems.Value;
            if (!string.IsNullOrEmpty(search))
            {
                if (notLike)
                {
                    string[] values = search.Split(',');
                    query = query.Where(o => values.Contains(o.Value.ToString()) || values.Contains(o.Name)).ToList();
                }
                else
                {
                    query = query.Where(o => o.Description.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1
                                            || o.Name.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) > -1).ToList();
                }
            }
            else if (notLike)
            {
                return result;
            }
            result = query.OrderBy(o => o.Name)
                .Select(o => new { o.Value, Name = o.Description ?? o.Name })
                .Select(o => new SelectItem() { key = o.Value.ToString(), value = o.Name })
                .ToList();
            return result;
        }
    }
}
