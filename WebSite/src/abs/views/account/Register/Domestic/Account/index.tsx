import * as React from 'react';
import { connect } from 'dva';
import { Form } from 'antd';
import { ABSInput } from '../../../../../../components/ABSInput';
import { ABSButton } from '../../../../../../components/ABSButton';
import { ABSSelect } from '../../../../../../components/ABSSelect';
import PasswordInput from '../../../../../../components/ABSInput/passwordInput';
import { ABSAntIcon } from '../../../../../../components/ABSAntIcon';
import ABSMessage from '../../../../../../components/ABSMessage';

const FormItem = Form.Item;
const passwordPattern = /^[a-zA-Z0-9]{6,16}$/;
const userNamePattern = /^[a-z0-9_.]{3,16}$/;

class Account extends React.Component<any, any> {
  compareToFirstPassword = (rule, value, callback) => {
    const form = this.props.form;
    if (value && value !== form.getFieldValue('password')) {
      callback('两次密码输入不一致');
    } else {
      callback();
    }
  }

  onSubmit = (e) => {
    e.preventDefault();
    const { form, onClick, dispatch } = this.props;
    form.validateFields((err, values) => {
      if (!err) {
        if (values.user_name && values.user_name !== '') {
          dispatch({
            type: 'account/checkUserName',
            payload: {
              user_name: values.user_name
            }
          }).then((response) => {
            if (response) {
              if (response.is_success) {
                onClick(values);
              } else {
                ABSMessage.error(response.fail_msg);
              }
            }
          });
        } else {
          onClick(values);
        }
      }
    });
  }

  render() {
    const { form, channelData, defaultSelectValue } = this.props;
    const { getFieldDecorator } = form;

    return (
      <div className="abs-domestic-account">
        <Form onSubmit={this.onSubmit} className="abs-domestic-form">
          <FormItem label="密码设置">
            {getFieldDecorator('password', {
              rules: [{
                required: true, whitespace: true, message: '请输入密码'
              }, {
                pattern: passwordPattern, message: '密码格式有误'
              }],
            })(
              <PasswordInput
                className={'abs-domestic-input'}
                placeholder="密码长度为6-16位字母或数字"
                prefix={<ABSAntIcon type="lock" />}
              />
            )}
          </FormItem>
          <FormItem label="确认密码">
            {getFieldDecorator('confirm_password', {
              rules: [{
                required: true, whitespace: true, message: '请输入确认密码'
              }, {
                pattern: passwordPattern, message: '密码格式有误'
              }, {
                validator: this.compareToFirstPassword,
              }],
            })(
              <PasswordInput
                className={'abs-domestic-input'}
                placeholder="密码长度为6-16位字母或数字"
                prefix={<ABSAntIcon type="lock" />}
              />
            )}
          </FormItem>
          <FormItem label="登录用户名">
            {getFieldDecorator('user_name', {
              rules: [{
                pattern: userNamePattern, message: '用户名格式有误'
              }]
            })(
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="用户名长度为3-16位，字母、数字、下划线、英文点"
                type="text"
                prefix={<ABSAntIcon type="user" />}
              />
            )}
          </FormItem>
          <FormItem label="了解CNABS的方式">
            {getFieldDecorator('channel', { initialValue: defaultSelectValue })(
              <ABSSelect
                value={defaultSelectValue}
                className={'abs-domestic-select'}
                optionData={channelData}
              />
            )}
          </FormItem>
          <FormItem className="form-item-submit">
            <ABSButton htmlType="submit" className="abs-register-btn">下一步</ABSButton>
          </FormItem>
        </Form>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global };
}

export default connect(mapStateToProps)(Form.create()(Account)); 