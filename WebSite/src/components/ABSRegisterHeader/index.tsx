import * as React from 'react';
import classNames from 'classnames';
import ABSUnLogin from './ABSUnLogin';
import { withRouter } from 'dva/router';
import './index.less';

const logo = require('../../assets/images/logo.png');

class ABSRegisterHeader extends React.Component<any, any> {
  render() {
    const { className, } = this.props;
    const classes = classNames('abs-register-header', className);
   
    return (
      <div className={classes}>
        <div className="abs-header-logo">
          <a href="/">
            <img src={logo} />
          </a>
        </div>
        <div className="abs-header-right">
          <ABSUnLogin />
        </div>
      </div>
    );
  }
}

export default withRouter(ABSRegisterHeader);