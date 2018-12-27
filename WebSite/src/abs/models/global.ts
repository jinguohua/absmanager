import Request from '../../utils/http/request/index';
import authUtil from '../../utils/authUtil';
import routeConfig from '../config/routeConfig';
import { GlobalApi, AccountApi, ProductApi, HomePageApi } from '../config/api';

export default {
  namespace: 'global',
  state: {
    user: {
      id: '',
      name: '',
      avatar: '',
      isNew: false
    },
    menu: {
      homeMenus: [], // 宣传页菜单
      siteMenus: [], // 顶部栏菜单
      personMenu: {}, // 个人中心菜单
      navigationMenus: [] // 侧边栏菜单
    },
    notice: {
      messageCount: 0 // 未读消息数量
    },
    version: '', // 系统版本号
    searchResult: {
      count: {
        deal: 0,
        security: 0,
        organization: 0,
        expert: 0
      },
      pager: {
        deal: {
          current: 1,
          total: 10,
          pageSize: 10,
          hideOnSinglePage: true,
          data: []
        },
        security: {
          current: 1,
          total: 10,
          pageSize: 10,
          hideOnSinglePage: true,
          data: []
        },
        organization: {
          current: 1,
          total: 10,
          pageSize: 10,
          hideOnSinglePage: true,
          data: []
        },
        expert: {
          current: 1,
          total: 10,
          pageSize: 10,
          hideOnSinglePage: true,
          data: []
        }
      }
    },
    homePageDetail: {}, // 首页信息
    params: {
      // 产品ID
      dealID: null,
      // 机构ID
      organizationID: null,
      // 证券ID
      securityID: null,
    }
  },
  effects: {
    // 获取权限
    *getAuth({ payload }: any, { call, put, select }: any) {
      // 获取用户信息
      let user = authUtil.getCurrentUser();
      if (!user) {
        const auth = yield call(Request.get, AccountApi.auth);
        if (auth) {
          authUtil.setAuthCache(auth);
          user = authUtil.getCurrentUser();
        }
      }

      // 用户信息过期时跳转登陆页
      const isLogin = authUtil.checkIsLogin();
      if (!isLogin) {
        authUtil.removeAllCache();
        location.href = routeConfig.login + '?returnurl=' + window.location.href;
        return;
      }

      // 获取菜单信息
      const menu = authUtil.getMenus();

      const data = { user, menu };
      yield put({
        type: 'updateAuth',
        payload: data,
      });
    },
    // 退出登录
    *logout(_: any, { call, put }: any) {
      const response = yield call(Request.post, AccountApi.logout);
      if (response) {
        authUtil.removeAllCache();
        location.href = routeConfig.login;
      }
    },
    *getUnauthMenusAndVersion(_: any, { call, put }: any) {
      const homeMenus = yield call(Request.post, GlobalApi.unauthMenu);
      if (homeMenus) {
        const { menus, version } = homeMenus;
        const data = {
          menu: {
            homeMenus: menus ? menus.map((item) => {
              return authUtil.parseSiteMenu(item);
            }) : []
          },
          version: version
        };

        yield put({
          type: 'updateUnauthMenusAndVersion',
          payload: data,
        });
      }
    },
    // 搜索
    *search({ payload }: any, { call, put }: any) {
      return yield call(Request.post, GlobalApi.search, null, payload);
    },
    // 分页搜索
    *searchByPage({ payload }: any, { call, put }: any) {
      const response = yield call(Request.post, GlobalApi.searchByPage, payload);
      if (response) {
        yield put({
          type: 'updateSearchResult',
          result: { response, payload },
        });
      }
    },
    // 筛选框的信息
    *filter({ payload }: any, { call, put }: any) {
      return yield call(Request.post, GlobalApi.filter, payload);
    },
    *getHomePageDetail({ payload }: any, { call, put }: any) {
      const response = yield call(Request.post, HomePageApi.homePage, payload);
      yield put({
        type: 'updateHomePageDetail',
        payload: response,
      });
    },
    *initParams({ payload }: any, { call, put }: any) {
      const params = authUtil.getParamsCache();
      if (params) {
        yield put({
          type: 'updateParams',
          payload: params,
        });
      }
    },
    *getScenarios({ payload }: any, { call, put, select }: any) {
      // const dealID = yield select(state => state.params.dealID);
      return yield call(Request.post, ProductApi.scenarioList, payload);
    },
  },
  reducers: {
    updateAuth(state: any, { payload }: any) {
      return {
        ...state,
        user: payload.user,
        menu: payload.menu,
      };
    },
    updateUnauthMenusAndVersion(state: any, { payload }: any) {
      return {
        ...state,
        menu: payload.menu,
        version: payload.version
      };
    },
    updateSearchResult(state: any, { result }: any) {
      const { response, payload } = result;
      const { search_type } = payload;
      const { searchResult } = state;
      const searchResultTemp: any = { count: response.count, deal: searchResult.deal, security: searchResult.security, organization: searchResult.organization, expert: searchResult.expert };
      switch (Number(search_type)) {
        case 1:
          searchResultTemp.deal = response.pager;
          return {
            ...state,
            searchResult: searchResultTemp
          };
        case 2:
          searchResultTemp.security = response.pager;
          return {
            ...state,
            searchResult: searchResultTemp
          };
        case 3:
          searchResultTemp.organization = response.pager;
          return {
            ...state,
            searchResult: searchResultTemp
          };
        case 4:
          searchResultTemp.expert = response.pager;
          return {
            ...state,
            searchResult: searchResultTemp
          };
        default: return {
          ...state
        };
      }
    },
    updateHomePageDetail(state: any, { payload }: any) {
      return {
        ...state,
        homePageDetail: payload,
      };
    },
    updateParams(state: any, { payload }: any) {
      return {
        ...state,
        params: payload.params,
      };
    },
    resetParams(state: any, { payload }: any) {
      const { params } = state ? state : '';
      const { dealID, organizationID, securityID } = params ? params : '';
      return {
        ...state,
        params: {
          dealID,
          organizationID,
          securityID: payload.securityID ? payload.securityID : securityID,
        },
      };
    },
  },

  subscriptions: {
    setup({ dispatch, history }: any) {
      return history.listen(({ pathname, search }) => {
        // 基本分析子页面切换
        if (pathname === '/security/info') {
          const pattern = new RegExp('[[\\]/?=]');
          const arr = search.split(pattern);
          const securityID = arr[2];
          dispatch({
            type: 'global/resetParams',
            payload: {
              securityID: securityID,
            },
          });
        }
      });
    },
  },
};