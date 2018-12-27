import React, { Component } from 'react';
import ABSRadio from '../../../../components/ABSRadio';
import { Row, Col } from 'antd';
import ABSDescription from '../../../../components/ABSDescription';

class ABSUIRadio extends Component {
  render() {
    return (
      <div>
        <ABSDescription>单选</ABSDescription>
        <Row style={{ marginBottom: 12}}>
          <Col span={2}><ABSRadio content="默认" value="0" /></Col>
          <Col span={2}><ABSRadio content="未选中禁止" value="3" disabled={true} /></Col>
        </Row>

        <Row style={{ marginBottom: 12}}>
          <Col span={2}> <ABSRadio content="hover" value="1" /></Col>
          <Col span={2}><ABSRadio content="选中禁用" value="4" checked={true} disabled={true} /></Col>
        </Row>

        <Row style={{ marginBottom: 12}}>
          <Col span={2}>
            <ABSRadio content="选中" value="2" checked={true} />
          </Col>
        </Row>
      </div>
    );
  }
}

export default ABSUIRadio;