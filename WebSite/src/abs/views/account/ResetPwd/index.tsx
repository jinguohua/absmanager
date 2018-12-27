import React from 'react';
import ResetProcess from './ResetProcess';
import './index.less';

export interface IResetPasswordProps {
  
}
 
export interface IResetPasswordState {
  
}
 
class ResetPassword extends React.Component<IResetPasswordProps, IResetPasswordState> {
  render() { 
    return (
      <div className="reset-password">
        <div className="reset-password-title">重置密码</div>
        <ResetProcess />
      </div>
    );
  }
}
 
export default ResetPassword;