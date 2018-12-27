import * as React from 'react';
import './index.less';
import { ABSAntIcon } from '../../../../../../components/ABSAntIcon';
import ABSLink from '../../../../../../components/ABSLink';
import RouterConfig from '../../../../../../abs/config/routeConfig';

export default class RegisterSuccess extends React.Component<any, any> {
  constructor(props: any) {
    super(props);
    this.state = {
      second: 5,
    };
  }

  render() {

    return (
      <div className="abs-reset-password-success">
        <div className="abs-reset-password-success-title">
          <ABSAntIcon className="abs-ant-icon-l" type="check-circle" theme="filled" style={{ color: '#52C41A' }} />
          <span className={'abs-reset-password-success-title-span'}> 密码设置成功</span>
        </div>
        <div className="abs-reset-password-success-content">
          请妥善保管好您的密码，回到<ABSLink to={RouterConfig.login} className="abs-reset-password-href">CNABS登录</ABSLink>。
        </div>
      </div>
    );
  }
}