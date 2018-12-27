import * as React from 'react';
import { Router, Route, Switch } from 'dva/router';
import routerUtil from '../../../utils/routerUtil';
import { Redirect } from 'react-router';
import identity  from '../../config/routerIdentityConfig';
import { menuKey } from '../../config/navigationMenuKeyConfig';

/**
 * App Component|Router Component
 */
export class App extends React.Component<any, any> {
  render() {
    const { app }: any = this.props;
    const routerConfig = {
      '/': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account', 'home'], () => import('../../../layouts/BasicLayout')),
      },
      '/login': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['account'], () => import('../../views/account/Login')),
      },
      '/main': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../../layouts/AnonymousLayout')),
      },
      '/main/register': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../views/account/Register')),
      },
      '/main/resetpwd': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['account'], () => import('../../views/account/ResetPwd')),
      },
      '/success': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../../layouts/MessageLayout')),
      },
      '/success/register': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../views/account/Register/Overseas/RegisterSuccess')),
      },
      '/register': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../views/account/Register')),
      },
      '/success/resetpwd': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../views/account/ResetPwd/ResetProcess/FinishResetProcess')),
      },
      '/success/modifypassword': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../views/account/ModifyPwd')),
      },
      '/user': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../../layouts/SideBarLayout')),
      },
      // 修改密码
      '/user/modify-password': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['global', 'account'], () => import('../../views/account/ModifyPwd')),
      },
    };

    const Login = routerConfig['/login'].component;
    const RegisterHeaderContentLayout = routerConfig['/main'].component;
    const BasicHeaderContentLayout = routerConfig['/success'].component;
    const HeaderContentLayout = routerConfig['/'].component;

    const UserHeaderContentLayout = routerConfig['/user'].component;
    return (
      <Router history={this.props.history}>
        <Switch>
          <Route path={'/user'} component={props => <UserHeaderContentLayout menuKey={menuKey.account} routerConfig={routerConfig} removePadding={true} {...props} />} />
          <Route path={'/main'} component={props => <RegisterHeaderContentLayout routerConfig={routerConfig} {...props} />} />
          <Route path={'/success'} component={props => <BasicHeaderContentLayout routerConfig={routerConfig} {...props} />} />
          <Route path={'/login'} component={props => <Login routerConfig={routerConfig} {...props} />} />
          <Redirect exact={true} path="/" to="/home" />
          <Route path={'/'} component={props => <HeaderContentLayout routerConfig={routerConfig} {...props} />} />
        </Switch>
      </Router>
    );
  }
}

/**
 * Dva Model Router Component Register and Bind
 * Every Router must set the model and component bind
 * use webpack chunk code split, you can set the chunk like the below
 * *webpackChunkName:'home'*
 * @param param0 
 */
export function RouterConfig({ history, app }: any) {
  return (
    <App history={history} app={app} />
  );
}  
