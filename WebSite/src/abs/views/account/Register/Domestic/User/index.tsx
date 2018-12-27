import * as React from 'react';
import { connect } from 'dva';
import { Form } from 'antd';
import { ABSInput } from '../../../../../../components/ABSInput';
import { ABSButton } from '../../../../../../components/ABSButton';
import './index.less';
import { ABSUploadAvatar } from '../../../../../../components/ABSUploadAvatar';
import ABSMessage from '../../../../../../components/ABSMessage';
import ABSRangeDatePicker from '../../../../../../components/ABSRangeDatePicker';
import moment from 'moment';
// import { ABSDatepicker } from '../../../../../../components/ABSDatepicker';
import routerConfig from '../../../../../config/routeConfig';
import ABSAutoComplete from '../../../../../../components/ABSAutoComplete';
import { AutoComplete } from 'antd';
import authUtil from '../../../../../../utils/authUtil';

const FormItem = Form.Item;
const Option = AutoComplete.Option;

interface IUserState {
  fileCode: string | undefined;
  startValue?: moment.Moment;
  endValue?: moment.Moment;
  organizations: any;
  organization_id: number;
  workingTimeError: string;
}

class User extends React.Component<any, IUserState> {
  constructor(props: any) {
    super(props);
    this.state = {
      fileCode: undefined,
      startValue: undefined,
      endValue: undefined,
      organizations: [],
      organization_id: 0,
      workingTimeError: ''
    };
  }

  handleFileCode = fileCode => {
    this.setState({ fileCode: fileCode });
  }

  onStartDateChange = (value) => {
    this.setState({
      startValue: value,
      workingTimeError: ''
    });
  }

  onEndDateChange = (value) => {
    this.setState({
      endValue: value,
      workingTimeError: ''
    });
  }

  handleSearch = (value) => {
    if (value !== '') {
      this.props.dispatch({
        type: 'global/search',
        payload: {
          keyword: value,
          search_type: 3,
        },
      }).then((response) => {
        if (response) {
          this.setState({ organizations: response });
        }
      });
    }
  }

  onChange = (value) => {
    const { organizations } = this.state;
    const selectedRow = organizations.filter((row) => row.title === value);
    if (selectedRow[0]) {
      this.setState({ organization_id: selectedRow[0].id });
    }
  }

  checkWorkingTime = (rule, value, callback) => {
    // const { startValue } = this.state;
    // if (!startValue) {
    //   callback('开始时间不能为空');
    // } else {
    //   callback();
    // }
  }

  onSubmit = (e) => {
    e.preventDefault();
    this.props.form.validateFields((err, values) => {
      const { startValue, endValue, organization_id, fileCode } = this.state;

      if (!fileCode) {
        ABSMessage.error('请上传头像');
        return;
      }

      if (!startValue) {
        this.setState({ workingTimeError: '请输入开始时间' });
        return;
      }

      if (!endValue) {
        this.setState({ workingTimeError: '请输入结束时间' });
        return;
      }

      if (!err) {
        const { phoneForm, accountForm } = this.props;
        var tempObject = {
          avatar: fileCode,
          working_time_start: startValue ? startValue.format('YYYY-MM-DD') : null,
          working_time_end: endValue ? endValue.format('YYYY-MM-DD') : null,
          organization_id
        };
        const params = Object.assign(values, phoneForm, accountForm, tempObject);

        this.props.dispatch({
          type: 'account/registerDomestic',
          payload: params
        }).then((response) => {
          if (!response) {
            return;
          }
          if (response.is_success) {
            authUtil.removeAllCache();
            location.href = routerConfig.home;
          } else {
            ABSMessage.error(response.fail_msg);
          }
        });
      }
    });
  }

  renderOption(item: any) {
    return (
      <Option key={item.title} title={item.title}>
        {item.title}
      </Option>
    );
  }

  render() {
    const { form } = this.props;
    const { getFieldDecorator } = form;
    const { startValue, endValue, organizations, workingTimeError } = this.state;

    return (
      <div className="abs-domestic-user">
        <ABSUploadAvatar updateFileCode={this.handleFileCode} />
        <Form onSubmit={this.onSubmit} className="abs-domestic-form">
          <FormItem label="真实姓名">
            {getFieldDecorator('name', {
              rules: [{ required: true, message: '请输入您的真实姓名' }],
            })(
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="请输入您的真实姓名"
                type="text"
              />
            )}
          </FormItem>
          <FormItem label="机构名称">
            {getFieldDecorator('organization_name', {
              rules: [{ required: true, message: '请输入或选择您的机构名称' }],
            })(
              <ABSAutoComplete
                placeholder={'请输入或选择您的机构名称'}
                dataSource={organizations.map(this.renderOption)}
                onSearch={this.handleSearch}
                onChange={this.onChange}
              />
            )}
          </FormItem>
          <FormItem label="所在部门">
            {getFieldDecorator('department')(
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="请输入您的所在部门"
                type="text"
              />
            )}
          </FormItem>
          <FormItem label="担任职位">
            {getFieldDecorator('position', {
              rules: [{ required: true, message: '请输入您的担任职位' }],
            })(
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="请输入您的担任职位"
                type="text"
              />
            )}
          </FormItem>
          {/* <FormItem label="在职时间">
            {getFieldDecorator('working_time', {
              // rules: [{ validator: this.checkWorkingTime }],
            })(
              <ABSRangeDatePicker
                startValue={startValue}
                endValue={endValue}
                onStartDateChange={this.onStartDateChange}
                onEndDateChange={this.onEndDateChange}
                splitStyle={{ width: 14, margin: '0 3px' }}
                isRequired={true}
                hideTitle={true}
                size="large"
              />
            )}
          </FormItem> */}
          <div className={'abs-form-item'}>
            <div className={'abs-form-item-lable'}>
              <span className={'abs-form-item-lable-danger'}>*</span> 在职时间
            </div>
            <div className={'abs-form-item-error'}>
              {workingTimeError}
            </div>
            <div className={'abs-form-item-content'}>
              <ABSRangeDatePicker
                startValue={startValue}
                endValue={endValue}
                onStartDateChange={this.onStartDateChange}
                onEndDateChange={this.onEndDateChange}
                splitStyle={{ width: 14, margin: '0 3px' }}
                isRequired={true}
                hideTitle={true}
                size="large"
              />
            </div>
          </div>
          <FormItem label="邮箱">
            {getFieldDecorator('email', {
              rules: [{
                required: true, message: '请输入您的邮箱'
              }, {
                type: 'email', message: '邮箱格式不正确'
              }],
            })(
              <ABSInput
                className={'abs-domestic-input'}
                placeholder="请输入您的邮箱"
                type="text"
              />
            )}
          </FormItem>
          <FormItem className="form-item-submit">
            <ABSButton htmlType="submit" className="abs-register-btn">完成，立即体验CNABS</ABSButton>
          </FormItem>
        </Form>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.account };
}

export default connect(mapStateToProps)(Form.create()(User)); 