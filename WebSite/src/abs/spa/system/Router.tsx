import * as React from 'react';
import { Router, Route, Switch } from 'dva/router';
import routerUtil from '../../../utils/routerUtil';
import identity  from '../../config/routerIdentityConfig';
import { menuKey } from '../../config/navigationMenuKeyConfig';
import ABSProductPageTitle from '../../../components/ABSProductPageTitle';

export class App extends React.Component<any, any> {
  render() {
    const { app }: any = this.props;
    const routerConfig = {
      '/list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'global', 'system'], () => import('../../../layouts/BasicLayout')),
      },
      '/list/user': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'system'], () => import('../../views/system/user/Index')),
      }
    };

    const BaseLayout = routerConfig['/list'].component;
    return (
      <Router history={this.props.history}>
        <Switch>
          <Route path="/list" component={props => <BaseLayout 
            menuKey={menuKey.projectEdit} 
            routerConfig={routerConfig} 
            pageHeader={<ABSProductPageTitle />}
            prefix="/list"
            {...props} />} />
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
