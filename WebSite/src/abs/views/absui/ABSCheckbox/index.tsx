import React, { Component } from 'react';
import ABSCheckbox from '../../../../components/ABSCheckbox';
import { Row, Col } from 'antd';
import ABSDescription from '../../../../components/ABSDescription';

class ABSCUIheckbox extends Component {
  render() {
    return (
      <div>
        <ABSDescription>多选</ABSDescription>
        <Row style={{ marginBottom: 12}}>
          <Col span={2}><ABSCheckbox content="默认" value="0" onChange={() => console.log('出发了onChange事件')} /></Col>
          <Col span={2}><ABSCheckbox content="未选中禁止" value="3" disabled={true} /></Col>
        </Row>

        <Row style={{ marginBottom: 12}}>
          <Col span={2}><ABSCheckbox content="hover" value="1" /></Col>
          <Col span={2}><ABSCheckbox content="选中禁用" value="4" checked={true} disabled={true} /></Col>
        </Row>

        <Row style={{ marginBottom: 12}}>
          <Col span={2}>
            <ABSCheckbox content="选中" value="2" checked={true} />
          </Col>
        </Row>
      </div>
    );
  }
}

export default ABSCUIheckbox;