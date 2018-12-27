import React, { Component } from 'react';
import { ABSAntIcon } from '../../../../components/ABSAntIcon';
import ABSIcon from '../../../../components/ABSIcon';
import { Row, Col } from 'antd';
import ABSDescription from '../../../../components/ABSDescription';

class ABSIconRoute extends Component {
  render() {
    return (
      <div>
        <ABSDescription>ABSAntIcon用于ant.design图标的展示</ABSDescription>
        <Row style={{ marginBottom: 8 }}>
          <Col span={1}><ABSAntIcon className="abs-ant-icon-s" type="edit" /></Col>
          <Col span={1}><ABSAntIcon className="abs-ant-icon-s" type="check-circle" theme="filled" style={{ color: '#FC9023'}} /></Col>
        </Row>
        <Row style={{ marginBottom: 8 }}>
          <Col span={1}><ABSAntIcon className="abs-ant-icon-l" type="edit" /></Col>
          <Col span={1}><ABSAntIcon className="abs-ant-icon-l" type="check-circle" theme="filled" style={{ color: '#FC9023'}} /></Col>
        </Row>

        <ABSDescription style={{ marginTop: 32 }}>ABSIcon用于font-cop图标的展示</ABSDescription>
        <Row>
          <Col span={1}><ABSIcon type="budget"/></Col>
          <Col span={1}><ABSIcon type="mail-filled" /></Col>
        </Row>
      </div>
    );
  }
}

export default ABSIconRoute;