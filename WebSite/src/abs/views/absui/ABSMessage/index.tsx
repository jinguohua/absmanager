import React, { Component } from 'react';
import ABSMessage from '../../../../components/ABSMessage';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';
import { Row } from 'antd';
import './index.less';
import { ABSButton } from '../../../../components/ABSButton';

class ABSMessageRoute extends Component {
  success = () => {
    ABSMessage.success('这是一条成功提示');
  }
  error = () => {
    ABSMessage.error('这是一条失败提示');
  }
  warning = () => {
    ABSMessage.warning('这是一条警告提示');
  }
  render () {
    return (
      <div className="absui-message">
        <ABSDescription>自动消失提示（一般应用于页面浮层提示，约3s自动消失）</ABSDescription>
        <Row style={{ marginBottom: 12}}>
          <ABSButton onClick={this.success}>成功提示</ABSButton>
        </Row>

        <Row style={{ marginBottom: 12}}>
          <ABSButton onClick={this.error} type="danger">失败提示</ABSButton>
        </Row>
        
        <Row style={{ marginBottom: 12}}>
          <ABSButton onClick={this.warning} type="default">警告提示</ABSButton>
        </Row>
      </div>
    );
  }
}

export default ABSMessageRoute;