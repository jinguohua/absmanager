import * as React from 'react';
import './index.less';
import ABSSteps from '../../../../../components/ABSSteps';
import Phone from '../../Register/Domestic/Phone';
import SetNewPassword from './SetNewPassword';
import { routerRedux } from 'dva/router';
import { connect } from 'dva';
import ResetPasswordCaptcha from './ResetPasswordCaptcha';
import ABSMessage from '../../../../../components/ABSMessage';

interface IState {
  phoneForm: object;
  accountForm: object;
}

export class ResetProcess extends React.Component<any, IState> {
  step: ABSSteps | null;

  phoneNumber: string;
  password: string;
  vcode: string;

  onNext = (formParams) => {
    const { dispatch } = this.props;

    if (this.step) {
      // this.step.next();
      const current = this.step.next();

      if (current === 1) {
        this.phoneNumber = formParams.mobile;
        this.vcode = formParams.vcode;
      }

      if (current === 2) {
        this.password = formParams.password;

        if (this.phoneNumber && this.password) {
          dispatch({
            type: 'account/resetPassword',
            payload: {
              mobile: this.phoneNumber,
              password: this.password,
              vcode: this.vcode,
            }
          }).then((response) => {
            if (response) {
              const { is_success } = response;
              if (is_success) {
                this.props.dispatch(routerRedux.push({
                  pathname: '/success/resetpwd',
                }));
              } else {
                ABSMessage.error(response.fail_msg);
              }
            }
          });
          return;
        }
        
      }
      // if (current === 1) {
      //   this.setState({phoneForm: formParams});
      // }
      // if (current === 2) {
      //   this.setState({accountForm: formParams});
      // }
    }
  }

  render() {
    const steps = [{
      title: '验证手机号',
      component: () => <ResetPasswordCaptcha onClick={this.onNext}/>,
    }, {
      title: '设置新密码',
      component: () => <SetNewPassword onClick={this.onNext}/>,
    }, {
      title: '完成设置',
      component: () => <Phone onClick={this.onNext}/>,
    }];

    return (
      <div className="abs-domestic">
        <ABSSteps className="abs-domestic-steps" ref={step => this.step = step} initial={0} steps={steps} />
      </div> 
    );
  }
}

const mapStateToProps = ({ global }) => {
  return { ...global };
};

export default connect(mapStateToProps)(ResetProcess);
