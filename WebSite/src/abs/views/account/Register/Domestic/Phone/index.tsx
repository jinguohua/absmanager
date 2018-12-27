import * as React from 'react';
import { connect } from 'dva';
import { Form } from 'antd';
import { ABSInput } from '../../../../../../components/ABSInput';
import { ABSButton } from '../../../../../../components/ABSButton';
import { ABSModal } from '../../../../../../components/ABSModal';
import ABSCheckbox from '../../../../../../components/ABSCheckbox';
import './index.less';
import Agreement from './Agreement';
import ABSMessage from '../../../../../../components/ABSMessage';
import ABSCountdown from '../../../../../../components/ABSCountdown';
import ABSIcon from '../../../../../../components/ABSIcon';

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
            type: 'account/getRegisterCaptcha',
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
          type: 'account/checkRegisterCaptcha',
          payload: values
        }).then((response) => {
          if (response) {
            if (response.is_success) {
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
    const phonePattern = /^1[345789]\d{9}$/;
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
                prefix={<ABSIcon type="mobilephone" />}
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
        <div className="abs-domestic-phone-aggrement">
          <ABSCheckbox content="勾选代表你已阅读并同意" value="2" checked={true} onChange={this.onChange} />
          <span className="abs-domestic-phone-aggrement-modal"><a onClick={this.onOpenAgreement}>《用户协议》</a></span>
        </div>
        <ABSModal
          content={<Agreement />}
          title={'《用户协议》'}
          width={800}
          footer={null}
          ref={view => this.modal = view}
        />
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.account };
}

export default connect(mapStateToProps)(Form.create()(Phone)); 