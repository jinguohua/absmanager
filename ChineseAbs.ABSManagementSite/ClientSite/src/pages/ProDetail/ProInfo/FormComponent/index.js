import React from 'react';
import moment from 'moment';
import { Button } from 'antd';
import WrappedHorizontalLoginForm from './FormLayout/index';

const dateFormat = 'YYYY-MM-DD';

// 表单组件
class FormComponent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      fields: {
        username: {
          value: 'benjycui',
        },
        type: {
          value: 'jack',
        },
        amoon: {
          value: true,
        },
        bmoon: {
          value: false,
        },
        time: {
          value: moment('2015-06-07', dateFormat),
        },
      },
    };
  }

  // 表单数据存储在上层组件中
  handleFormChange = changedFields => {
    this.setState(({ fields }) => ({
      fields: { ...fields, ...changedFields },
    }));
  };

  sendInfo = () => {
    let { fields: value } = this.state;
    value = { ...value, time: { value: value.time.value.format('YYYY-MM-DD') } };

    console.log(value);
  };
  // 表单数据存储在上层组件中

  render() {
    const { fields } = this.state;
    return (
      <div>
        <WrappedHorizontalLoginForm {...fields} onChange={this.handleFormChange} />
        {/* <WrappedHorizontalLoginForm {...fields} /> */}
        <Button onClick={this.sendInfo}>提交</Button>
      </div>
    );
  }
}

export default FormComponent;
