import React from 'react';
import { Route, Switch } from 'dva/router';
import RenderAuthorized from 'ant-design-pro/lib/Authorized';
import routerUtil from '../../utils/routerUtil';
import authUtil from '../../utils/authUtil';
import ABSNotFound from '../ABSNotFound';

const Authorized = RenderAuthorized('root');
const { AuthorizedRoute }: any = Authorized;

export interface IProps {
  routerConfig: object;
  path: string;
}

export class ABSAuthorizedRouteList extends React.Component<IProps, any> {
  notFound = () => (
     <ABSNotFound />
  )

  render() {
    const { routerConfig, path } = this.props;

    return (
      <Switch>
        {
          routerUtil.getRoutes(path, routerConfig).map(item => {
            return (
              <AuthorizedRoute
                key={item.key}
                path={item.path}
                component={item.component}
                exact={item.exact}
                authority={() => { return authUtil.hasPermission(item.identity); }}
              />
            );
          })
        }
        <Route render={this.notFound} />
      </Switch>
    );
  }
}
