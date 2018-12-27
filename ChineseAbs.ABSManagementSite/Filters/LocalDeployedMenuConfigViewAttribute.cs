using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Utils;
//using ChineseAbs.Authorize;
//using ChineseAbs.Authorize.HtmlTmpl;
//using ChineseAbs.Authorize.Service;
using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;

namespace ChineseAbs.ABSManagementSite.Filters
{
    //public class LocalDeployedMenuConfigViewAttribute : ActionFilterAttribute
    //{
    //    public LocalDeployedMenuConfigViewAttribute()
    //    {
    //        //本地版菜单存放在文件中
    //        MenuList = GetAllMenu();
    //        AuthMenuList = GetAuthMenu();
    //    }
    //    public override void OnResultExecuting(ResultExecutingContext filterContext)
    //    {
    //        if (filterContext.Result.GetType().IsAssignableFrom(typeof(ViewResult)))
    //        {
    //            bool isLogin = filterContext.HttpContext.User.Identity.IsAuthenticated;
    //            var account = isLogin ? filterContext.HttpContext.User.Identity.Name : "";

    //            var result = (ViewResult)filterContext.Result;

    //            var model = new MenuViewModel();
    //            UserState userState = null;

    //            if (isLogin)
    //            {
    //                userState = CreateUserStateFromDB(account);
    //                if (userState == null)
    //                    isLogin = false;
    //                else
    //                    isLogin = true;
    //            };

    //            var userID = userState == null ? account : userState.UserId;
    //            //用户昵称处理
    //            var nickName = userState == null ? account : userState.Name;
    //            //用户简称处理
    //            string simpleName = GetSimpleName(nickName);
    //            //头像路径处理
    //            var avatar = UserState.UserDefaultAvatar;

    //            model.UserInfo = new UserInfo
    //            {
    //                IsLogin = isLogin,
    //                Avatar = avatar,
    //                Name = nickName,
    //                SimpleName = simpleName,
    //                ID = userID,
    //                IshasAuth = false
    //            };

    //            model.UserState = userState;

    //            MenuConfig menuConfig = AuthMenuService.GetMenuConfig();
    //            model.MenuConfig = menuConfig;
    //            //读取缓存菜单权限
    //            List<MenuNode> menuTree = null;
    //            if (isLogin)
    //            {
    //                menuTree = GetUserMenuTree(userState);
    //            }
    //            else
    //            {
    //                menuTree = GetAllMenuTree();
    //            }

    //            var context = System.Web.HttpContext.Current;
    //            var url = GetRegexPatternMenuLinkUrl(context);
    //            //处理SiteMap和本地测试链接替换
    //            var authMenuService = new AuthMenuService();
    //            var siteMap = GetSiteMap(menuTree, url, menuConfig);

    //            //设置站点SiteMap
    //            model.SiteMap = siteMap;
    //            //设置分离功能菜单与个人中心菜单
    //            model.SiteMenu = authMenuService.GetSiteMenuTree(menuTree);
    //            model.PersonMenu = authMenuService.GetPersonMenuTree(menuTree);


    //            var service = IocContainer.Resolve<AuthMenuService>();
    //            var menuKey = PassCodeCookieHelper.GetCacheItem<string>("LastMenuKey", false);

    //            if (model.SiteMap.IsSuccess)
    //                PassCodeCookieHelper.SetCacheItem("LastMenuKey", model.SiteMap.Third.MenuKey);

    //            result.ViewBag.IsLogin = model.UserInfo.IsLogin;
    //            result.ViewBag.MenuHtml = GetMenuHtml(model);
    //            result.ViewBag.SiteMapHtml = CnabsHtmlHelper.GetSiteMapHtml(model);
    //            result.ViewBag.FooterHtml = GetFooterHtml();
    //        }
    //    }

    //    /// <summary>
    //    /// 处理功能部分菜单特殊逻辑
    //    /// </summary>
    //    /// <param name="userState"></param>
    //    /// <param name="menuList"></param>
    //    private void InitSiteMenuTree(UserState userState, List<MenuNode> menuList)
    //    {
    //        if (userState == null)
    //            return;

    //        //判断是否是管理员
    //        if (!Roles.IsUserInRole(userState.Account, RoleAdmin))
    //        {
    //            menuList.Remove(menuList.Where(r => r.MenuKey == "menuLinkDurationSetting").FirstOrDefault());
    //            menuList.Remove(menuList.Where(r => r.MenuKey == "menuLinkManage").FirstOrDefault());
    //        }

    //        List<MenuNode> authList = AuthMenuList;
    //        foreach (var item in menuList)
    //        {
    //            if (item.IsFree)
    //            {
    //                continue;
    //            }

    //            if (authList.Where(o => o.ID == item.ID).Any())
    //                item.IsOpen = false;
    //        }

    //        //打开存续期管理平台子菜单权限
    //        SetSpecialMenuAuth(menuList, authList, "DurationManagePlatform");
    //    }

    //    /// <summary>
    //    /// 获取菜单Html
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <returns></returns>
    //    public static string GetMenuHtml(MenuViewModel model)
    //    {
    //        var tmplService = new RazorTmplService();
    //        var html = tmplService.GetTmplHtml("MenuTmpl", model);
    //        return html;
    //    }

    //    /// <summary>
    //    /// 获取页脚Html
    //    /// </summary>
    //    /// <returns></returns>
    //    public static string GetFooterHtml()
    //    {
    //        var service = IocContainer.Resolve<AuthMenuService>();
    //        var model = new Footer { VersionNo = SysUtils.GetDllVersion(), MenuConfig = service.GetConfig() };
    //        var tmplService = new RazorTmplService();
    //        var html = tmplService.GetTmplHtml("FooterTmpl", model);
    //        return html;
    //    }

    //    public UserState CreateUserStateFromDB(string userName)
    //    {
    //        if (string.IsNullOrEmpty(userName))
    //            return null;

    //        var item = new DBAdapter().LocalDeployed.GetUserProfile(userName);

    //        UserState userState = new UserState();
    //        if (item != null)
    //        {
    //            userState.Account = item.UserName;
    //            userState.UserId = Convert.ToString(item.Id);
    //            userState.UserName = item.UserName;
    //            userState.Name = item.RealName;
    //            userState.Telephone = string.Empty;
    //            userState.CellPhone = item.Cellphone;
    //            userState.Department = item.Department;
    //            userState.Company = item.Company;
    //            userState.CompanyAddress = string.Empty;
    //            userState.Email = item.Email;
    //            userState.Wechat = string.Empty;

    //            //获取用户角色

    //            userState.Org = new OrgProperty();
    //            userState.Org.IsOrgAccount = false;

    //            userState.ManagerRole = new ManagerRole();
    //            userState.ManagerRole.Admin = false;
    //            userState.ManagerRole.Analyst = false;
    //            userState.ManagerRole.CompanyUser = false;
    //            userState.ManagerRole.LabUser = false;
    //            userState.ManagerRole.Marketing = false;
    //            userState.ManagerRole.OrganizationUser = false;
    //            userState.ManagerRole.Ticket = false;

    //            return userState;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }

    //    public SiteMapMenu GetSiteMap(List<MenuNode> menuTree, string menuUrl, MenuConfig menuConfig)
    //    {
    //        //匹配菜单  
    //        var siteMap = GetSiteMapMenu(menuTree, menuUrl, menuConfig);
    //        return siteMap;
    //    }

    //    /// <summary>
    //    /// 处理链接匹配的正则表达式
    //    /// </summary>
    //    /// <param name="linkUrl"></param>
    //    /// <returns></returns>
    //    private string GetUrlRegex(string linkUrl)
    //    {
    //        //解析端口处理
    //        linkUrl = linkUrl.ToLower();
    //        Uri urlPath;
    //        if (linkUrl.ToLower().StartsWith("http"))
    //            urlPath = new Uri(linkUrl);
    //        else
    //        {
    //            if (linkUrl.StartsWith("//"))
    //                urlPath = new Uri("http:" + linkUrl);
    //            else
    //                return string.Empty;
    //        }


    //        //判断是否使用Nginx代理,导致内网转换请求带端口规则
    //        var urlRex = linkUrl.Replace("https://", "//").Replace("http://", "//");
    //        urlRex = Regex.Escape(urlRex); //处理正则转义符
    //        //if (this._menuConfig.IsNginxAgent)
    //        //{
    //        //    if (urlPath.Port == 80 || urlPath.Port == 443)
    //        //        urlRex = urlRex.Replace(Regex.Escape(urlPath.Host), Regex.Escape(urlPath.Host + ":") + @"(\d{1,5})?");
    //        //    else
    //        //        urlRex = urlRex.Replace(Regex.Escape(":" + urlPath.Port), Regex.Escape(":") + @"(\d{1,5})?");
    //        //}

    //        return urlRex;
    //    }

    //    private SiteMapMenu GetSiteMapMenu(List<MenuNode> menuTree, string urlOrMenuKey, MenuConfig menuConfig)
    //    {
    //        SiteMapMenu siteMapMenu = new SiteMapMenu();
    //        siteMapMenu.IsSuccess = false;

    //        foreach (var itemL1 in menuTree)
    //        {
    //            //替换测试链接
    //            itemL1.LinkUrl = ReplaceDomainMenuUrl(itemL1.LinkUrl, menuConfig);

    //            itemL1.IsCurrent = false;
    //            foreach (var itemL2 in itemL1.Children)
    //            {
    //                //替换测试链接
    //                itemL2.LinkUrl = ReplaceDomainMenuUrl(itemL2.LinkUrl, menuConfig);

    //                itemL2.IsCurrent = false;
    //                foreach (var itemL3 in itemL2.Children)
    //                {
    //                    if (!itemL3.IsOpen)
    //                    {
    //                        continue;
    //                    }
    //                    //替换测试链接
    //                    itemL3.LinkUrl = ReplaceDomainMenuUrl(itemL3.LinkUrl, menuConfig);

    //                    itemL3.IsCurrent = false;
    //                    if (string.IsNullOrEmpty(urlOrMenuKey) || itemL3.LinkUrl == null || itemL3.MenuKey == null)
    //                    {
    //                        continue;
    //                    }

    //                    var urlRex = GetUrlRegex(itemL3.LinkUrl);
    //                    if (string.IsNullOrEmpty(urlRex))
    //                        continue;

    //                    if (((urlOrMenuKey == itemL3.MenuKey || Regex.IsMatch(urlOrMenuKey, urlRex))
    //                        && !IsManageConflict(urlOrMenuKey, itemL3.MenuKey, urlRex))
    //                        || (urlOrMenuKey.Contains("task") && itemL3.MenuKey == "menuLinkSchedule"))
    //                    {
    //                        itemL3.IsCurrent = true;
    //                        itemL2.IsCurrent = true;
    //                        itemL1.IsCurrent = true;

    //                        siteMapMenu.Third = itemL3;
    //                        siteMapMenu.Second = itemL2;
    //                        siteMapMenu.First = itemL1;
    //                        siteMapMenu.IsSuccess = true;
    //                    }
    //                }
    //            }
    //        }

    //        return siteMapMenu;
    //    }

    //    private bool IsManageConflict(string urlOrMenuKey, string itemLMenuKey, string urlRex)
    //    {
    //        return (!urlOrMenuKey.Contains("profile") || !itemLMenuKey.Contains("profile"))
    //            && (!urlOrMenuKey.Contains("profile") || !urlRex.Contains("profile"))
    //            && (urlOrMenuKey.Contains("profile") || itemLMenuKey.Contains("profile"))
    //            && (urlOrMenuKey.Contains("profile") || urlRex.Contains("profile"));
    //    }

    //    private string ReplaceDomainMenuUrl(string linkUrl, MenuConfig menuConfig)
    //    {
    //        foreach (var domain in menuConfig.TestMenuConfig.DomainConfig)
    //        {
    //            if (!String.IsNullOrEmpty(linkUrl))
    //            {
    //                if (linkUrl.ToLower().Contains(domain.From.ToLower()))
    //                {
    //                    linkUrl = linkUrl.Replace(domain.From.ToLower(), domain.To.ToLower());
    //                }
    //            }
    //        }
    //        return linkUrl;
    //    }



    //    private static string GetRegexPatternMenuLinkUrl(System.Web.HttpContext context)
    //    {
    //        var url = context.Request.Url.AbsoluteUri.ToLower().Replace("https://", "//").Replace("http://", "//");
    //        //url =  Regex.Escape(url); 
    //        //var port = context.Request.Url.Port; //:9000
    //        //url = url.Replace(":" + port, @"(:\d{1,5})?"); //.Replace("https:","").Replace("http:","")
    //        ////      .Replace("?",@"\?").Replace(".",@"\.").Replace(":" + 9000, @"(:\d{1,5})?");
    //        return url;
    //    }

    //    private string GetSimpleName(string nickName)
    //    {
    //        var simpleName = "";
    //        if (!string.IsNullOrEmpty(nickName) && nickName.Length > 3)
    //        {
    //            simpleName = nickName.Substring(0, 2) + "...";
    //        }
    //        else
    //        {
    //            simpleName = nickName;
    //        }

    //        return simpleName;
    //    }

    //    public List<MenuNode> GetAllMenuTree()
    //    {
    //        var menuTree = CacheHelper.Get<List<MenuNode>>(UserState.AllMenuTreeKey);
    //        if (menuTree != null)
    //            return menuTree;

    //        var allMenuList = MenuList;
    //        var cnabsList = GetCnabsMenu(allMenuList);
    //        //设置"存续期设置"菜单权限
    //        cnabsList.Remove(cnabsList.Where(r => r.MenuKey == "menuLinkDurationSetting").FirstOrDefault());
    //        //移除个人中心菜单
    //        cnabsList.Remove(cnabsList.Where(r => r.MenuKey == "PersonCenter").FirstOrDefault());

    //        //构建菜单树
    //        menuTree = CreateMenuTree(cnabsList);
    //        CacheHelper.Set(UserState.AllMenuTreeKey, menuTree, UserState.UserStateTimeOut);
    //        return menuTree;
    //    }

    //    public List<MenuNode> GetCnabsMenu(List<MenuNode> allMenuList)
    //    {
    //        var query = allMenuList.Where(o => o.IsAdmin == false);
    //        if (query.Any())
    //            return query.ToList();
    //        else
    //            return new List<MenuNode>();
    //    }

    //    #region 构建菜单树
    //    /// <summary>
    //    /// 构建菜单树
    //    /// </summary>
    //    /// <returns></returns>
    //    private List<MenuNode> CreateMenuTree(List<MenuNode> menuList)
    //    {
    //        var rootID = GetMenuNodeID(menuList, "CNABS");
    //        var menus = menuList.Where(r => r.ParentID == rootID);

    //        var categries = new List<MenuNode>();
    //        foreach (var item in menus)
    //        {
    //            item.Level = 1;
    //            item.Children = GetSubMenuList(item, menuList);
    //            categries.Add(item);
    //        }

    //        return categries;
    //    }

    //    /// <summary>
    //    /// 递归构建菜单树节点
    //    /// </summary>
    //    /// <param name="parentID">菜单父ID</param>
    //    /// <param name="menuList">菜单集合</param>
    //    /// <returns></returns>
    //    public List<MenuNode> GetSubMenuList(MenuNode parentNode, List<MenuNode> menuList)
    //    {
    //        bool isFirstNode = true;
    //        var children = new List<MenuNode>();
    //        try
    //        {
    //            var subMenuList = menuList.Where(r => r.ParentID == parentNode.ID);
    //            foreach (var item in subMenuList)
    //            {
    //                item.Level = parentNode.Level + 1;
    //                item.Children = GetSubMenuList(item, menuList);

    //                //标记新菜单
    //                if (item.Children == null || item.Children.Count() == 0)
    //                    FindParentNodeSetIsNew(menuList, item);

    //                //标记层级菜单的首个链接
    //                if (isFirstNode)
    //                {
    //                    parentNode.LinkUrl = item.LinkUrl;
    //                    isFirstNode = false;
    //                }
    //                children.Add(item);
    //            }

    //        }
    //        catch
    //        {

    //        }

    //        return children;
    //    }

    //    private void FindParentNodeSetIsNew(List<MenuNode> menuList, MenuNode childNode)
    //    {
    //        if (childNode.IsNew)
    //        {
    //            var parentNode = menuList.Where(o => o.ID == childNode.ParentID).FirstOrDefault();
    //            if (parentNode != null)
    //            {
    //                parentNode.IsNew = true;
    //                FindParentNodeSetIsNew(menuList, parentNode);
    //            }
    //        }
    //    }

    //    private void FindParentNodeSetIsMapSite(List<MenuNode> menuList, MenuNode childNode)
    //    {
    //        if (childNode.IsNew)
    //        {
    //            var parentNode = menuList.Where(o => o.ID == childNode.ParentID).FirstOrDefault();
    //            if (parentNode != null)
    //            {
    //                parentNode.IsNew = true;
    //                FindParentNodeSetIsNew(menuList, parentNode);
    //            }
    //        }
    //    }

    //    #endregion

    //    /// <summary>
    //    /// 用户已登录的菜单树
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<MenuNode> GetUserMenuTree(UserState userState)
    //    {
    //        var menuList = this.GetUserMenuList(userState);
    //        //构建菜单树
    //        var menuTree = this.CreateMenuTree(menuList);
    //        return menuTree;
    //    }

    //    public List<MenuNode> GetUserMenuList(UserState userState)
    //    {
    //        string key = UserState.UserMenuListKey + userState.Account.ToLower();
    //        var cnabsList = CacheHelper.Get<List<MenuNode>>(key);
    //        if (cnabsList == null)
    //        {
    //            var allMenuList = MenuList;
    //            cnabsList = this.GetCnabsMenu(allMenuList);
    //            //对菜单特殊逻辑处理
    //            InitSiteMenuTree(userState, cnabsList);
    //            InitPersonMenuTree(userState, cnabsList, allMenuList);
    //            CacheHelper.Set(key, cnabsList, UserState.UserStateTimeOut);
    //        }

    //        return cnabsList;
    //    }


    //    /// <summary>
    //    /// 获取个人中心菜单特殊逻辑(主要处理管理菜单逻辑)
    //    /// </summary>
    //    /// <returns></returns>
    //    private void InitPersonMenuTree(UserState userState, List<MenuNode> cnabsList, List<MenuNode> allMenuList)
    //    {
    //        if (userState == null)
    //            return;

    //        var orgMenuList = GetOrgMenu(allMenuList);
    //        var cnabsMenuList = GetAdminMenu(allMenuList);

    //        var role = userState.ManagerRole;

    //        //设置机构用户菜单权限
    //        if (userState.Org.IsOrgAccount)
    //        {
    //            foreach (var item in orgMenuList)
    //            {
    //                //非专业版时不显示“个人中心-机构账号管理-机构文件管理/机构私有产品”菜单
    //                if (userState.Org.OrgTypeID != 3)
    //                {
    //                    if (item.MenuKey == "OrgReport" || item.MenuKey == "OrgPrivateDeal")
    //                    {
    //                        continue;
    //                    }
    //                }

    //                cnabsList.Add(item);
    //            }
    //        }

    //        //设置CNABS内部管理菜单权限
    //        foreach (var item in cnabsMenuList)
    //        {

    //            if (item.MenuKey == "CNABSManage")
    //            {
    //                if (role.Admin || role.Analyst || role.Marketing)
    //                    cnabsList.Add(item);

    //            }

    //            if (role.Admin)
    //            {
    //                if (item.MenuKey == "Index" || item.MenuKey == "NewsAudit")
    //                {
    //                    cnabsList.Add(item);

    //                }
    //            }

    //            if (role.Analyst)
    //            {
    //                if (item.MenuKey == "AnalystIndex")
    //                {
    //                    cnabsList.Add(item);
    //                }
    //            }

    //            if (role.Marketing)
    //            {
    //                if (item.MenuKey == "MarketIndex")
    //                {
    //                    cnabsList.Add(item);
    //                }
    //            }
    //        }

    //        //设置工单管理菜单权限
    //        if (!role.Ticket)
    //        {
    //            cnabsList.Remove(cnabsList.Where(r => r.MenuKey == "ReceiveTicket").FirstOrDefault());
    //            cnabsList.Remove(cnabsList.Where(r => r.MenuKey == "SelfTicketList").FirstOrDefault());
    //        }
    //    }

    //    /// <summary>
    //    /// 设置特殊的菜单权限
    //    /// </summary>
    //    /// <param name="menuList">菜单列表</param>
    //    /// <param name="openList">有权限的菜单列表</param>
    //    /// <param name="DealOnlineDesignID">菜单ID</param>
    //    private void SetSpecialMenuAuth(List<MenuNode> menuList, List<MenuNode> openList, string menuKey)
    //    {
    //        var menuID = GetMenuNodeID(menuList, menuKey);
    //        if (openList.Where(o => o.ID == menuID).Any())
    //        {
    //            foreach (var item in menuList.Where(r => r.ParentID == menuID))
    //            {
    //                item.IsOpen = true;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 获取CNABS内部管理菜单列表
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<MenuNode> GetAdminMenu(List<MenuNode> allMenuList)
    //    {
    //        var parentID = GetMenuNodeID(allMenuList, "CNABSManage");
    //        var query = allMenuList.Where(o => o.ID == parentID || o.ParentID == parentID);
    //        if (query.Any())
    //            return query.ToList();
    //        else
    //            return new List<MenuNode>();
    //    }

    //    /// <summary>
    //    /// 获取机构账号菜单列表
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<MenuNode> GetOrgMenu(List<MenuNode> allMenuList)
    //    {

    //        var parentID = GetMenuNodeID(allMenuList, "OrgManage");
    //        var query = allMenuList.Where(o => o.ID == parentID || o.ParentID == parentID);
    //        if (query.Any())
    //            return query.ToList();
    //        else
    //            return new List<MenuNode>();
    //    }

    //    /// <summary>
    //    /// 获取菜单根节点ID
    //    /// <param name="menuKey">菜单KEY</param>
    //    /// </summary>
    //    /// <returns></returns>
    //    public int GetMenuNodeID(List<MenuNode> allMenuList, string menuKey)
    //    {
    //        var node = allMenuList.Where(o => o.MenuKey == menuKey).FirstOrDefault();
    //        if (node == null)
    //            return 0;
    //        else
    //            return node.ID;
    //    }

    //    private List<MenuNode> GetAllMenu()
    //    {
    //        var allMenuList = new List<MenuNode>();

    //        var path = startPath + "MenuLocalDeployed/localDeployedMenu.txt";
    //        StreamReader sr = new StreamReader(path, Encoding.Default);
    //        string line;
    //        line = sr.ReadLine();
    //        while ((line = sr.ReadLine()) != null)
    //        {
    //            var cells = line.ToString().Split('\t');
    //            var menu = new MenuNode();
    //            menu.ID = int.Parse(cells[0]);
    //            menu.Name = cells[1];
    //            menu.MenuKey = cells[2];
    //            menu.IsFree = cells[6] == "NULL" || cells[6] == "0" ? false : true;
    //            if (menu.IsFree)
    //                menu.IsOpen = true;
    //            else
    //                menu.IsOpen = false;
    //            menu.LinkUrl = cells[4];
    //            menu.ParentID = cells[3] == "NULL" ? 0 : int.Parse(cells[3]);
    //            menu.SortIndex = cells[5] == "NULL" ? 0 : int.Parse(cells[5]);

    //            allMenuList.Add(menu);
    //        }

    //        return allMenuList;
    //    }

    //    /// <summary>
    //    /// 获取用户授权的菜单
    //    /// </summary>
    //    /// <param name="userName">用户名</param>
    //    /// <returns></returns>
    //    private List<MenuNode> GetAuthMenu()
    //    {
    //        var openList = new List<MenuNode>();

    //        var path = startPath + "MenuLocalDeployed/authMenu.txt";
    //        StreamReader sr = new StreamReader(path, Encoding.Default);
    //        string line;
    //        line = sr.ReadLine();
    //        while ((line = sr.ReadLine()) != null)
    //        {
    //            var cells = line.ToString().Split('\t');
    //            var menu = new MenuNode();
    //            menu.ID = int.Parse(cells[0]);
    //            menu.Name = cells[1];
    //            openList.Add(menu);
    //        }

    //        return openList;
    //    }

    //    internal static readonly string RoleAdmin = "Admin";
    //    public static readonly string startPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

    //    private List<MenuNode> MenuList { get; set; }
    //    private List<MenuNode> AuthMenuList { get; set; }
    //}
}
