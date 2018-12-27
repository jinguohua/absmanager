import * as React from 'react';
import { connect } from 'dva';
import { Form } from 'antd';
import { ABSTab } from '../../../../components/ABSTab';
import Overseas from './Overseas';
import './index.less';
import Domestic from './Domestic';

class Register extends React.Component<any, any> {
  render() {
    const panesList = [
      {
        title: '国内用户',
        key: '1',
        content: <Domestic />,
      },
      {
        title: '国外用户',
        key: '2',
        content: <Overseas />,
      },
    ];
 
    return (
     
      <div className="abs-register">
        <div className="abs-register-title">欢迎注册</div>
        <ABSTab activeKey={'1'} panesList={panesList} type="full" />
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.account };
}

export default connect(mapStateToProps)(Form.create()(Register)); 