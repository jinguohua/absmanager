import { createElement } from 'react';
import dynamic from 'dva/dynamic';
// import TopBarProgress from '../abs/config/topBarProgress';

/**
 * 路由配置封装
 */
export default class RouterUtil {
 
  /**
   * 生成路由dynamic
   * @param {any} app 当前app
   * @param {any} models models数组
   * @param {any} component 组件
   * @returns {dynamic} dynamic
   */
  static dynamicWrapper = (app: any, models: any, component: any) => {
    return dynamic({
      app,
      models: () => models.filter(
        model => RouterUtil.modelNotExisted(app, model)).map(m => import(`../abs/models/${m}`)
        ),
      // add routerData prop
      component: () => {
        return component().then((raw) => {
          const Component = raw.default || raw;
          return props => createElement(Component, {
            ...props,
          });
        });
      },
      // LoadingComponent: TopBarProgress
    });
  }

  static modelNotExisted = (app, model) => (
    // eslint-disable-next-line
    !app._models.some(({ namespace }) => {
      return namespace === model.substring(model.lastIndexOf('/') + 1);
    })
  )

  /**
   * 获取路由配置
   * { path:{name,...param}}=>Array<{name,path ...param}>
   * @param {string} path 当前Url路径
   * @param {routerData} routerData 路由数据
   * @returns {any} 路由
   */
  static getRoutes = (path: string, routerData: any) => {
    let routes = Object.keys(routerData).filter(routePath =>
      routePath.indexOf(path) === 0 && routePath !== path);
    // Replace path to '' eg. path='user' /user/name => name
    routes = routes.map(item => item.replace(path, ''));
    // Get the route to be rendered to remove the deep rendering
    const renderArr = RouterUtil.getRenderArr(routes);
    // Conversion and stitching parameters
    const renderRoutes = renderArr.map((item) => {
      const exact = !routes.some(route => route !== item && RouterUtil.getRelation(route, item) === 1);
      return {
        ...routerData[`${path}${item}`],
        key: `${path}${item}`,
        path: `${path}${item}`,
        exact,
      };
    });
    return renderRoutes;
  }

  private static getRenderArr = (routes: Array<any>) => {
    let renderArr = new Array<any>();
    renderArr.push(routes[0]);
    for (let i = 1; i < routes.length; i += 1) {
      let isAdd = false;
      // 是否包含
      isAdd = renderArr.every(item => RouterUtil.getRelation(item, routes[i]) === 3);
      // 去重
      renderArr = renderArr.filter(item => RouterUtil.getRelation(item, routes[i]) !== 1);
      if (isAdd) {
        renderArr.push(routes[i]);
      }
    }
    return renderArr;
  }

  private static getRelation = (str1: string, str2: string) => {
    if (str1 === str2) {
      console.warn('Two path are equal!');  // eslint-disable-line
    }
    const arr1 = str1.split('/');
    const arr2 = str2.split('/');
    if (arr2.every((item, index) => item === arr1[index])) {
      return 1;
    } else if (arr1.every((item, index) => item === arr2[index])) {
      return 2;
    }
    return 3;
  }
}