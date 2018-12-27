import * as React from 'react';
import { connect } from 'dva';
import { Form } from 'antd';
import { ABSInput } from '../../../../../../components/ABSInput';
import { ABSButton } from '../../../../../../components/ABSButton';
import ABSMessage from '../../../../../../components/ABSMessage';
import ABSCountdown from '../../../../../../components/ABSCountdown';
import { ABSAntIcon } from '../../../../../../components/ABSAntIcon';
import './index.less';

const FormItem = Form.Item;

interface IState {
  disabled: boolean;
}

class Phone extends React.Component<any, IState> {
  modal;
  constructor(props: any) {
    super(props);
    this.state = {
      disabled: true
    };
  }

  onGetCaptcha = async () => {
    const value = await this.getRegisterCaptcha();
    if (value) {
      return true;
    } else {
      return false;
    }
  }

  getRegisterCaptcha = () => {
    return new Promise((resolve, reject) => {
      this.props.form.validateFields(['mobile'], (err, values) => {
        if (!err) {
          this.props.dispatch({
            type: 'account/getResetPasswordCaptcha',
            payload: {
              mobile: values.mobile
            }
          }).then((response) => {
            if (response) {
              if (response.is_success) {
                ABSMessage.success('验证码发送成功！');
                this.setState({ disabled: false });
                resolve(true);
              } else {
                ABSMessage.error(response.fail_msg);
                resolve(false);
              }
            } else {
              resolve(false);
            }
          });
        } else {
          resolve(false);
        }
      });
    });
  }

  // onGetCaptcha = () => {
  //   const error = this.props.form.getFieldError('mobile');
  //   const value = this.props.form.getFieldValue('mobile');
    
  //   if (!error && value) {
  //     this.props.dispatch({
  //       type: 'account/getResetPasswordCaptcha',
  //       payload: {
  //         mobile: value
  //       }
  //     }).then((response) => {
  //       if (response) {
  //         if (response.is_success) {
  //           ABSMessage.success('验证码发送成功！');
  //           this.setState({ disabled: false });
  //         } else {
  //           ABSMessage.error(response.fail_msg);
  //         }
  //       }
  //     });

  //     return true;
  //   } else {
  //     return false;
  //   }
  // }

  onOpenAgreement = () => {
    this.modal.setState({ visible: true });
  }

  onChange = () => {
    ABSMessage.warning('登录、注册必须同意该协议！');
  }

  onSubmit = (e) => {
    e.preventDefault();
    this.props.form.validateFields((err, values) => {
      if (!err) {
        this.props.dispatch({
          type: 'account/checkResetPasswordCaptcha',
          payload: values
        }).then((response) => {
          if (response) {
            const { is_success } = response;
            if (is_success) {
              this.props.onClick(values);
            } else {
              ABSMessage.error(response.fail_msg);
            }
          }
        });
      }
    });
  }

  render() {
    const { disabled } = this.state;
    const { form } = this.props;
    const { getFieldDecorator } = form;
    const phonePattern = /^1[34578]\d{9}$/;
    return (
      <div className="abs-domestic-phone">
        <Form onSubmit={this.onSubmit} className="abs-domestic-form">
          <FormItem>
            {getFieldDecorator('mobile', {
              rules: [{
                required: true, message: '请输入手机号'
              }, {
                pattern: phonePattern, message: '手机格式有误'
              }],
            })(
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="请输入手机号"
                type="text"
                prefix={<ABSAntIcon type="mobilephone" />}
              />
            )}
          </FormItem>
          <FormItem className="form-item-captcha">
            {getFieldDecorator('vcode', {
              rules: [{
                required: true, message: '请输入验证码'
              }],
            })(
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="请输入验证码"
                type="text"
                disabled={disabled}
                addonAfter={<ABSCountdown onStartCountdown={this.onGetCaptcha} />}
              />
            )}
          </FormItem>
          <FormItem className="form-item-submit">
            <ABSButton htmlType="submit" className="abs-register-btn" >下一步</ABSButton>
          </FormItem>
        </Form>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.account };
}

export default connect(mapStateToProps)(Form.create()(Phone)); 