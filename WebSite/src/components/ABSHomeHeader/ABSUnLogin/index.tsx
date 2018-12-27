import * as React from 'react';
import { ABSButton } from '../../ABSButton';
import authUtil from '../../../utils/authUtil';
import './index.less';
import routerConfig from '../,,/../../../abs/config/routeConfig';

interface IProps {
  isLogin: boolean;
  history?: any;
}

class ABSUnLogin extends React.Component<IProps, any> {
  goLogin = () => {
    if (authUtil.checkIsLogin()) {
      location.href = routerConfig.home;
    } else {
      location.href = routerConfig.login;
    }
  }

  goRegister = () => {
    location.href = routerConfig.register;
  }
  
  render() {
    return (
      <div className="abs-unlogin">
        <ABSButton className="btn-logion" onClick={this.goLogin} >登录</ABSButton>
        {/* <ABSButton className="btn-register" type="default"  onClick={this.goRegister}>注册</ABSButton> */}
      </div>
    );
  }
}

export default ABSUnLogin; 