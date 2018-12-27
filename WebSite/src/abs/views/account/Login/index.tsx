import * as React from 'react';
import { connect } from 'dva';
import { Form } from 'antd';

const logoLogin = require('../../../../assets/images/logo-login.png');
import './index.less';
import { ABSInput } from '../../../../components/ABSInput';
import { ABSButton } from '../../../../components/ABSButton';
import { Link } from 'react-router-dom';
import ABSMessage from '../../../../components/ABSMessage';
import PasswordInput from '../../../../components/ABSInput/passwordInput';
import { ABSAntIcon } from '../../../../components/ABSAntIcon';
import routerConfig from '../../../config/routeConfig';
import commonUtils from '../../../../utils/commonUtils';
import authUtil from '../../../../utils/authUtil';
const FormItem = Form.Item;

class ABSLogin extends React.Component<any, any> {

  constructor(props: any) {
    super(props);
    this.state = {
      isChange: false,
      errorMessage: ''
    };
  }

  componentDidMount() {
    const params = commonUtils.getParams();
    if (params && params.message) {
      this.setState({ errorMessage: params.message });
    }
  }

  onGoHome = () => {
    location.href = routerConfig.introdution;
  }

  onDeleteBtn(e: any) {
    if (e.user_name) {
      this.props.form.setFieldsValue({
        user_name: '',
      });
    }
  }

  onSubmit = (e) => {
    e.preventDefault();
    this.setState({errorMessage: ''});
    this.props.form.validateFields((err, values) => {
      if (!err) {
        this.props.dispatch({
          type: 'account/login',
          payload: values
        }).then((response) => {
          if (response === '') {
            authUtil.removeAllCache();
            const params = commonUtils.getParams(); {
              if (params && params.return_url) {
                location.href = params.return_url;
              } else {
                location.href = routerConfig.home;
              }
            }
          } else {
            ABSMessage.error(response);
            this.setState({ isChange: !this.state.isChange });
          }
        });
      }
    });
  }

  render() {
    const { form } = this.props;
    const { getFieldDecorator } = form;
    const { errorMessage } = this.state;

    const userName = this.props.form.getFieldsValue(['user_name']);

    return (
      <div className="abs-login">
        <div className="abs-login-bg">
          <img className="abs-login-logo" src={logoLogin} />
        </div>
        <div className="abs-login-content">
          <div className="abs-login-content-backhome">
            <ABSButton className="btn-backhome" onClick={this.onGoHome}>回到主页</ABSButton>
          </div>
          <div className="abs-login-content-title">
            用户登录
          </div>
          <Form onSubmit={this.onSubmit} className="abs-login-form">
            <FormItem>
              {getFieldDecorator('UserName', {
                rules: [{ required: true, message: '请输入用户名' }],
              })(
                <ABSInput
                  placeholder="请输入用户名"
                  getvalue={userName}
                  deletBtn={() => this.onDeleteBtn(userName)}
                  issuffix={true}
                  prefix={<ABSAntIcon type="user" />}
                />
              )}
            </FormItem>
            <FormItem>
              {getFieldDecorator('password', {
                rules: [{ required: true, message: '请输入密码' }],
              })(
                <PasswordInput placeholder="请输入密码" prefix={<ABSAntIcon type="lock" />} />
              )}
            </FormItem>
            <FormItem className="form-item-submit">
              <span className={'form-item-submit-error'}>{errorMessage}</span>
              <ABSButton htmlType="submit" className="abs-login-btn" >登  录</ABSButton>
            </FormItem>
          </Form>
          <div className="abs-login-content-account">
            <span className="abs-login-content-account-resetpwd"><Link to={'/main/resetpwd'}>忘记密码</Link></span>
          </div>
        </div>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.account };
}

export default connect(mapStateToProps)(Form.create()(ABSLogin)); 