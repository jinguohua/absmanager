import * as React from 'react';
import { Router, Route, Switch, Redirect } from 'dva/router';
import routerUtil from '../../../utils/routerUtil';
import identity from '../../config/routerIdentityConfig';
import { menuKey } from '../../config/navigationMenuKeyConfig';
import ABSProductPageTitle from '../../../components/ABSProductPageTitle';

export class App extends React.Component<any, any> {
  render() {
    const { app }: any = this.props;
    const routerConfig = {
      '/detail': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'global'], () =>
          import('../../../layouts/PageHeaderLayout')
        )
      },
      '/list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(
          app,
          ['absui', 'global', 'project'],
          () => import('../../../layouts/BasicLayout')
        )
      }
    };

    const DetailLayout = routerConfig['/detail'].component;
    const BaseLayout = routerConfig['/list'].component;
    return (
      <Router history={this.props.history}>
        <Switch>
          <Redirect path="/" exact={true} to="/list/assets" />
          <Route
            path="/detail"
            component={props => (
              <DetailLayout
                menuKey={menuKey.projectEdit}
                routerConfig={routerConfig}
                pageHeader={<ABSProductPageTitle />}
                prefix="/detail"
                {...props}
              />
            )}
          />
          <Route
            path="/list"
            component={props => (
              <BaseLayout routerConfig={routerConfig} {...props} />
            )}
          />
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
  return <App history={history} app={app} />;
}
