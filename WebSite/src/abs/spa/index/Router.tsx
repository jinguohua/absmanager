import * as React from 'react';
import { Router, Route, Switch, Redirect } from 'dva/router';
import routerUtil from '../../../utils/routerUtil';
import identity  from '../../config/routerIdentityConfig';
import { menuKey } from '../../config/navigationMenuKeyConfig';

export class App extends React.Component<any, any> {
  render() {
    const { app }: any = this.props;
    const routerConfig = {
      '/': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'global'], () => import('../../../layouts/BasicLayout')),
      }, 
      '/main/home': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'global'], () => import('../../views/index/home')),
      }
    };

    const Home = routerConfig['/main/home'].component;

    const Layout = routerConfig['/'].component;
    return (
      <Router history={this.props.history}>
        <Switch>
          <Redirect path="/" exact={true} to="/main/home" />
          <Route path="/" component={props => <Layout menuKey={menuKey.absUI} routerConfig={routerConfig} {...props} />} />
          <Route path="/main/home" component={props => <Home />} />
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
