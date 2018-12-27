import * as React from 'react';
import { connect } from 'dva';
import { Form } from 'antd';
import { ABSInput } from '../../../../../../components/ABSInput';
import { ABSButton } from '../../../../../../components/ABSButton';
import './index.less';

const FormItem = Form.Item;
const passwordPattern = /^[a-z0-9]{6,16}$/;

interface IState {
  defaultSelectValue: string;
  selectOptionData: Array<string>;
}

class SetNewPassword extends React.Component<any, IState> {
  constructor(props: any) {
    super(props);
    this.state = {
      defaultSelectValue: '',
      selectOptionData: []
    };
  }

  componentDidMount() {
    this.props.dispatch({
      type: 'global/filter',
      payload: {
        filter_subtype_id: 21
      }
    }).then((response) => {
      if (response) {
        const selectItem = response.find(r => r.selected);
        this.setState({
          defaultSelectValue: selectItem ? selectItem.key : '',
          selectOptionData: this.parseData(response)
        });
      }
    });
  }

  parseData = (data) => {
    return data.map((item) => {
      return {
        value: item.key,
        labelName: item.value,
        className: 'abs-select'
      };
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

  onSubmit = (e) => {
    e.preventDefault();
    this.props.form.validateFields((err, values) => {
      if (!err) {
        this.props.onClick(values);
      }
    });
  }

  render() {
    const { form } = this.props;
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
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="密码长度为6-16位字母或数字"
                type="password"
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
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="密码长度为6-16位字母或数字"
                type="password"
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

export default connect(mapStateToProps)(Form.create()(SetNewPassword)); 