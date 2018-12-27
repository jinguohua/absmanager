import * as React from 'react';
import { Router, Route, Switch } from 'dva/router';
import routerUtil from '../../../utils/routerUtil';
import identity  from '../../config/routerIdentityConfig';

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
      '/list': {
          identity: identity.common
      }
    };

    const HeaderContentLayout = routerConfig['/'].component;
    return (
      <Router history={this.props.history}>
        <Switch>
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
