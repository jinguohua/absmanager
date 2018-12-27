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
      '/project': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(
          app,
          ['absui', 'global', 'project'],
          () => import('../../../layouts/PageHeaderLayout')
        )
      },
      '/project/base': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'project'], () =>
          import('../../views/projects/edit/BaseInfo')
        )
      },
      '/project/notes': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'project'], () =>
          import('../../views/projects/edit/Notes')
        )
      },
      '/project/accounts': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'project'], () =>
          import('../../views/projects/edit/Accounts')
        )
      },
      '/project/fees': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'project'], () =>
          import('../../views/projects/edit/Fees')
        )
      },
      '/projects': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(
          app,
          ['absui', 'global', 'project'],
          () => import('../../../layouts/BasicLayout')
        )
      },
      '/projects/list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(
          app,
          ['absui', 'global', 'project'],
          () => import('../../views/projects/list')
        )
      },
      '/duration': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(
          app,
          ['absui', 'global', 'project'],
          () => import('../../../layouts/BasicLayout')
        )
      }
    };

    const DetailLayout = routerConfig['/project'].component;
    const BaseLayout = routerConfig['/projects'].component;
    const DurationLayout = routerConfig['/duration'].component;
    return (
      <Router history={this.props.history}>
        <Switch>
          <Redirect path="/" exact={true} to="/projects/list" />
          <Route
            path="/project"
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
            path="/projects"
            component={props => (
              <BaseLayout routerConfig={routerConfig} {...props} />
            )}
          />
          <Route
            path="/duration"
            component={props => (
              <DurationLayout routerConfig={routerConfig} {...props} />
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
