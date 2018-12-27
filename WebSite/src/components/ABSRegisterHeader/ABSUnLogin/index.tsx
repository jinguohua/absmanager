import * as React from 'react';
import './index.less';
import routerConfig from '../../../abs/config/routeConfig';
import ABSLink from '../../ABSLink';

class ABSUnLogin extends React.Component<any, any> {
  render() {
    return (
      <div className="abs-unlogin">
        <span className={'hasLogin'}>已有CNABS登录账号?</span>
        <ABSLink to={routerConfig.login}>点击登录</ABSLink>
      </div>
    );
  }
}

export default ABSUnLogin; 