import React, { Component } from 'react';
import { Form } from 'antd';
import '../../../../styles/coverAnt.less';
import { ABSButton } from '../../../../components/ABSButton';
import './index.less';
import PasswordInput from '../../../../components/ABSInput/passwordInput';
import { ABSInput } from '../../../../components/ABSInput';
import ABSDescription from '../../../../components/ABSDescription';
// import commouUtils from '../../../../utils/commonUtils';

const FormItem = Form.Item;

const filteremail = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

interface IParticipateProjectState {
}

class ABSFrom extends Component<any, IParticipateProjectState> {
  constructor(props: any) {
    super(props);
    this.state = {
    };
  }

  // 自定义提示
  validateToNextPhoneNumber = (rule, value, callback) => {
    if (value && !(/^1[34578]\d{9}$/.test(value))) {
      callback('号码格式不对');
    } else {
      callback('');
    }
  }

  // 使用了form表单后，input的value值需要使用setFieldsValue来清空
  deletPhoneBtn(e: any) {
    if (e.form) {
      this.props.form.setFieldsValue({
        form: '',
      });
    }
  }
  deletemailBtn(e: any) {
    if (e.email) {
      this.props.form.setFieldsValue({
        email: '',
      });
    }
  }
  deletmonlieBtn(e: any) {
    if (e.mobile) {
      this.props.form.setFieldsValue({
        mobile: '',
      });
    }
  }

  onSubmit = (e) => {
    e.preventDefault();
    this.props.form.validateFieldsAndScroll((err, values) => {
      if (!err) {
        // 
      }
    });
  }
  // // 如果使用正则验证 可以使用onChang的回调 再外面再次验证
  // phoneonChange = (e) => {
  //   // const formText = this.props.form.getFieldValue('mobile');

  //   if (e && !(/^1[34578]\d{9}$/.test(e))) {
  //     this.setState({ isRightType: false });
  //   } else {
  //     this.setState({ isRightType: true });
  //   }
  // }

  render() {
    const { getFieldDecorator } = this.props.form;
    const formText = this.props.form.getFieldsValue();
    const phonePattern = /^1[34578]\d{9}$/;
    return (
      <div className="from-view">
        <Form onSubmit={this.onSubmit}>
          <ABSDescription>手机号码验证——"使用了自定义验证"</ABSDescription>
          <div className="from-item-view">
            <FormItem>
              {getFieldDecorator('form', {
                rules: [{
                  required: true, whitespace: true, message: '请输入手机号码',
                }, {
                  validator: this.validateToNextPhoneNumber,
                }],
              })(
                <ABSInput placeholder="请输入号码" getvalue={formText} inputType="form" deletBtn={() => this.deletPhoneBtn(formText)} style={{ width: 420 }} issuffix={true} regular={phonePattern}/>
              )}
            </FormItem>
          </div>

          <ABSDescription style={{ marginTop: 32 }}>密码验证</ABSDescription>
          <div className="from-view">
            <FormItem>
              {getFieldDecorator('password', {
                rules: [{
                  required: true, message: '请输入密码',
                }],
              })(
                <PasswordInput placeholder="请输入密码" style={{ width: 420 }} />
              )}
            </FormItem>
          </div>

          <ABSDescription style={{ marginTop: 32 }}>邮箱验证--"使用了自带的邮箱验证"</ABSDescription>
          <div className="from-view">
            <FormItem validateStatus="error">
              {getFieldDecorator('email', {
                rules: [{
                  type: 'email', message: '邮箱地址不对'
                }, {
                  required: true, message: '请输入邮箱地址',
                }],
              })(
                <ABSInput placeholder="请输入邮箱" getvalue={formText} inputType="email" deletBtn={() => this.deletemailBtn(formText)} style={{ width: 420 }} issuffix={true} regular={filteremail}/>
              )}
            </FormItem>
          </div>

          <ABSDescription style={{ marginTop: 32 }}>正则验证方法,使用onChange方法再次验证规则"</ABSDescription>
          <div className="from-view">
            <FormItem validateStatus="error">
              {getFieldDecorator('mobile', {
                rules: [{
                  pattern: phonePattern, message: '手机格式有误'
                }],
              })(
                <ABSInput
                  placeholder="请输入手机号"
                  style={{ width: 420 }}
                  // onChange={this.phoneonChange}
                  issuffix={true}
                  getvalue={formText}
                  inputType="mobile"
                  deletBtn={() => this.deletmonlieBtn(formText)}
                  regular={phonePattern}
                />
              )}
            </FormItem>
          </div>

          <ABSButton type="primary" htmlType="submit">提交</ABSButton>
        </Form>

      </div>
    );
  }
}

const WrappedRegistrationForm = Form.create()(ABSFrom);
export default WrappedRegistrationForm;