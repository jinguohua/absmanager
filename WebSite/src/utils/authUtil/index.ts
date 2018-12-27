import commonUtils from '../commonUtils';
import { menuKey } from '../../abs/config/navigationMenuKeyConfig';
import routeConfig from '../../abs/config/routeConfig';
import routerIdentityConfig from '../../abs/config/routerIdentityConfig';
import { absUIMenuData } from './absUIMenuData';
import { menusData } from '../../abs/config/menus';

export interface IUser {
  id: string;
  name: string;
  avatar: string;
  isLogin: boolean;
  isNew: boolean;
}

interface IToken {
  token: string;
  expireIn: number;
}

interface IMenu {
  id: number;
  parentId: number;
  name: string;
  key: string;
  url: string;
  children: IMenu[];
}

export interface ISiteMenu extends IMenu {
  isNew: boolean;
  isOpen: boolean;
  isFree: boolean;
  children: ISiteMenu[];
}

export interface INavigationMenu extends IMenu {
  icon?: string;
  siteMenuKey?: string;
}

interface IAuthMenu {
  siteMenus: ISiteMenu[];
  personMenu: ISiteMenu;
  navigationMenus: INavigationMenu[];
}

interface IAuth {
  user: IUser;
  token: IToken;
  menu: IAuthMenu;
}

interface IScenario {
  id: number;
  name: string;
}

interface IUrlParams {
  params: {
    // 产品ID
    dealID: number,
    // 机构ID
    organizationID: number,
    // 证券ID
    securityID: number,
  };
}

/**
 * 权限业务封装
 */
class AuthService {
  readonly userCacheKey: string = 'security.user';
  readonly menuCacheKey: string = 'security.menu';
  readonly tokenCacheKey: string = 'security.token';
  readonly paramsCacheKey: string = 'security.params';

  // 获取当前用户信息
  getCurrentUser(): IUser | null {
    const userData = localStorage.getItem(this.userCacheKey);
    if (!userData) { return null; }
    const user = JSON.parse(userData) as IUser;
    return user;
  }

  // 获取菜单信息
  getMenus(): any {
    const menuData = localStorage.getItem(this.menuCacheKey);
    if (!menuData) { return null; }
    let menus = JSON.parse(menuData) as IAuthMenu;
    return menus;
  }

  // 获取单个侧边栏菜单
  getNavigationMenu(navigationMenuKey: any, scenarios?: IScenario[], dealId?: number | null): any {
    const menus = this.getMenus();
    if (!menus) { return null; }
    let { navigationMenus } = menus;

    const currentMenu = navigationMenus.find((menu) => menu.key === navigationMenuKey);
    if (currentMenu) {
      const staticAnalysis = currentMenu.children.find(r => r.key === menuKey.dealStaticAnalysis);
      if (scenarios) {
        if (staticAnalysis && scenarios.length > 0) {
          scenarios.forEach((item) => {
            let menu: INavigationMenu = {
              id: item.id,
              parentId: staticAnalysis.id,
              name: item.name,
              key: item.name,
              url: `${routeConfig.dealStaticAnalysis}?scenario_id=${item.id}`,
              children: []
            };
            staticAnalysis.children.push(menu);
          });
        } else {
          currentMenu.children.splice(currentMenu.children.findIndex(r => r.key === menuKey.dealStaticAnalysis), 1);
        }
      }

      return currentMenu.children;
    }

    return [];
  }

  // 设置本地数据存储（包括：用户信息、菜单权限）
  setAuthCache(authData: any) {
    authData.menu = menusData;
    
    let navigationMenus = authData.menu.navigationMenus.map((item) => {
      return this.parseNavigationMenu(item);
    });
    
    const absUiMenu = { key: menuKey.absUI, children: absUIMenuData };
    navigationMenus.push(absUiMenu);

    let auth: IAuth = {
      user: this.parseUser(authData.user),
      token: this.parseToken(authData.token),
      menu: {
        siteMenus: authData.menu.siteMenus.map((item) => {
          return this.parseSiteMenu(item);
        }),
        personMenu: this.parseSiteMenu(authData.menu.personMenu),
        navigationMenus,
      }
    };

    // 1.设置当前用户信息
    localStorage.setItem(this.userCacheKey, JSON.stringify(auth.user));

    // 2.设置菜单信息
    localStorage.setItem(this.menuCacheKey, JSON.stringify(auth.menu));

    // 3.设置Token信息
    localStorage.setItem(this.tokenCacheKey, JSON.stringify(auth.token));

  }

  // 更新用户缓存
  updateUserCache(userState: IUser) {
    localStorage.setItem(this.userCacheKey, JSON.stringify(userState));
  }

  // 清除本地数据存储（包括：用户信息、菜单权限）
  removeAllCache() {
    localStorage.removeItem(this.userCacheKey);
    localStorage.removeItem(this.menuCacheKey);
    localStorage.removeItem(this.tokenCacheKey);
  }

  /**
   * 判断用户是否登录
   */
  checkIsLogin() {
    const token = this.getToken();

    // 判断token是否过期
    if (!token) { return false; }
    const ts = new Date() as any - 0;

    if (token.expireIn > ts) {
      return true;
    }

    return false;
  }

  /**
   * 检查权限
   * @param {string} [identity] 
   */
  hasPermission(identity: string): boolean {
    // 权限令牌不存在,禁止放行
    if (!identity) {
      location.href = routeConfig.home;
      return true;
    }

    // 通用页面，直接放行
    if (identity === routerIdentityConfig.common) {
      return true;
    }

    // 权限不存在，禁止放行
    const menu = this.getMenus();
    if (!menu) {
      location.href = routeConfig.home;
      return false;
    }

    // 权限令牌不匹配，禁止放行
    const siteMenus = menu.siteMenus;
    if (!this.checkAuth(identity, siteMenus)) {
      location.href = routeConfig.home;
      return false;
    }
    
    return true;
  }

  /**
   * 检查权限
   * @param {string} [identity] 
   */
  checkAuth(identity: string, menus: ISiteMenu[]): boolean {
    for (let i = 0; i < menus.length; i++) {
      let menu = menus[i];
      if (identity === menu.key && menu.isOpen === true) { return true; }
      if (menu.children && menu.children.length > 0 && this.checkAuth(identity, menu.children)) {
        return true;
      } else {
        continue;
      }
    }

    return false;
  }

  parseSiteMenu(menu: any) {

    let siteMenu: ISiteMenu = {
      id: menu.id,
      name: menu.name,
      key: menu.key,
      url:  commonUtils.parseUrl(menu.url),
      isNew: menu.isNew,
      isOpen: menu.isOpen,
      isFree: menu.isFree,
      parentId: menu.parentId,
      children: menu.children.map((item) => {
        return this.parseSiteMenu(item);
      }),
    };

    return siteMenu;
  }

  getSubMenu(menus: Array<IMenu>, key: menuKey) {
    const subMenu = menus.find((menu) => menu.key === key);
    if (subMenu) {
      return subMenu.children;
    }
    return [];
  }

  getParamsCache() {
    let urlParams = commonUtils.getParams();
    if (urlParams && JSON.stringify(urlParams) !== '{}') {
      urlParams = this.parseUrlParams(urlParams);
      localStorage.setItem(this.paramsCacheKey, JSON.stringify(urlParams));
      return urlParams;
    }
    const params = localStorage.getItem(this.paramsCacheKey);
    if (!params) { return null; }
    return JSON.parse(params);
  }

  private parseUrlParams(data: any) {
    let returnData: IUrlParams = {
      params: {
        dealID: data.deal_id ? data.deal_id : null,
        organizationID: data.organization_id ? data.organization_id : null,
        securityID: data.security_id ? data.security_id : null,
      }
    };

    return returnData;
  }
  /**
   * 获取Token
   */
  private getToken(): IToken | null {
    const tokenStr = localStorage.getItem(this.tokenCacheKey);
    if (!tokenStr) { return null; }
    const token = JSON.parse(tokenStr) as IToken;

    return token;
  }

  private parseNavigationMenu(menu: any) {
    let siteMenu: INavigationMenu = {
      id: menu.id,
      name: menu.name,
      key: menu.key,
      url: commonUtils.parseUrl(menu.url),
      icon: menu.icon,
      parentId: menu.parentId,
      siteMenuKey: menu.siteMenuKey,
      children: (menu.children || []).map((item) => {
        return this.parseNavigationMenu(item);
      }),
    };

    return siteMenu;
  }

  private parseUser(userState: any) {
    const user: IUser = {
      id: userState.id,
      name: userState.name,
      avatar: commonUtils.getAvatar(userState.avatar),
      isLogin: true,
      isNew: false,
    };

    return user;
  }

  private parseToken(token: any) {
    const data: IToken = {
      token: token.token,
      expireIn: token.expireIn,
    };

    return data;
  }
}

export default new AuthService();
