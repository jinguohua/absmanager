import React from 'react';
import './index.less';
import { ABSInput } from '../../../../components/ABSInput';
import ABSPanel from '../../../../components/ABSPanel';
import { ABSButton } from '../../../../components/ABSButton';
import { Form } from 'antd';
import { connect } from 'dva';
import  ModifySuccess from './ModifySuccess';
import ABSMessage from '../../../../components/ABSMessage';

const FormItem = Form.Item;
// const passwordPattern = /^[a-z0-9]{6,16}$/;

export interface IABSNewsListProps {
  dispatch: any;
}

export interface IModifyPasswordState {
  newpassword: string;
  oldpassword: string;
  nextpassword: string;
  hidestuts: boolean;
}

class ModifyPwd extends React.Component<any, IModifyPasswordState> {
  constructor(props: any) {
    super(props);
    this.state = {
      newpassword: '',
      oldpassword: '',
      nextpassword: '',
      hidestuts: false,
    };
  }
  oldinputChange = (e) => {
    this.setState({ oldpassword: e});
  }

  newinputChange = (e) => {
    this.setState({ newpassword: e});
  }

  nextinputChange = (e) => {
    this.setState({ nextpassword: e});
  }

  confirmOnClick = (e) => {
    this.props.form.validateFieldsAndScroll((err, values) => {
      const oldpassword = values.oldpassword;
      const password = values.password;
      const confirm_password = values.confirm_password;
      if (!err) {
        this.props.dispatch({
          type: 'account/getModifyPwd',
          payload: { OldPassword: oldpassword, NewPassword: password, RepeatPassword: confirm_password},
        }).then((response) => {
          if (!response) {
            return;
          }
          const { is_success, fail_msg } = response;
          if (is_success) {
            this.setState({ hidestuts: true});
            // ABSMessage.success('修改密码成功');
            // window.location.href = `/account.html#/login`;
            return;
          }
          ABSMessage.success(fail_msg);
        });
       
      }
    });
  }

  compareToFirstPassword = (rule, value, callback) => {
    const form = this.props.form;
    if (value && value !== form.getFieldValue('password')) {
      callback('两次密码输入不一致');
    } else {
      callback();
    }
  }

  render() {
    const { oldpassword, newpassword, nextpassword, hidestuts } = this.state;
    const { getFieldDecorator } = this.props.form;
    if (hidestuts) {
      return <ModifySuccess/>;
    }
    return (
      <div className="modify-password">
        <ABSPanel title="修改密码">
          <Form onSubmit={this.confirmOnClick} className="abs-login-form">
            <div className="old-password">
              <div className="title-view">旧密码</div>
              <FormItem  >
                {getFieldDecorator('oldpassword', {
                  rules: [{
                    required: true, whitespace: true, message: '请输入密码'
                  }],
                })(
                  <ABSInput
                    className="small-view"
                    placeholder="请输入旧密码"
                    size="default"
                    onChange={this.oldinputChange}
                    value={oldpassword}
                    type="password"
                  />
                )}
              </FormItem>
            </div>

            <div className="new-password">
              <div className="title-view">新密码</div>
              <FormItem  >
                {getFieldDecorator('password', {
                  rules: [{
                    required: true, whitespace: true, message: '请输入新密码'
                  }, ],
                })(
                  <ABSInput
                    className="small-view"
                    placeholder="密码长度为6-16位字母或数字"
                    size="default"
                    onChange={this.newinputChange}
                    value={newpassword}
                    type="password"
                  />
                )}
              </FormItem>
            </div>

            <div className="repeat-password">
              <div className="title-view">重复新密码</div>

              <FormItem  >
                {getFieldDecorator('confirm_password', {
                  rules: [{
                    required: true, whitespace: true, message: '请输入确认密码'
                  }, {
                    validator: this.compareToFirstPassword,
                  }],
                })(
                  <ABSInput
                    className="small-view"
                    placeholder="请输入确认密码"
                    size="default"
                    onChange={this.nextinputChange}
                    value={nextpassword}
                    type="password"
                  />
                )}
              </FormItem>
            </div>

            <div className="repeat-password">
              <div className="title-view" />
              <div className="button-view">
                <ABSButton onClick={this.confirmOnClick} icon="check" >确定</ABSButton>
              </div>
            </div>

          </Form>
        </ABSPanel>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global };
}

export default connect(mapStateToProps)(Form.create()(ModifyPwd)); 