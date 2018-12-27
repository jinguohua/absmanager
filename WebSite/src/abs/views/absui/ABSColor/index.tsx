import React from 'react';
import ABSColorPalette from './ABSColorPalette';
import { Row, Col } from 'antd';

export interface IABSColorProps {
  
}
 
class ABSUIColor extends React.Component<IABSColorProps> {
  
  render() { 
    return (
      <div className="abs-color">
        <Row>
          <ABSColorPalette color="#ffffff" />
        </Row>
        <Row>
          <Col span={4}>
            <ABSColorPalette color="#ffffff" />
          </Col>
          <Col span={4}>
            <ABSColorPalette color="#ffffff" />
          </Col>
          <Col span={4}>
            <ABSColorPalette color="#ffffff" />
          </Col>
          <Col span={4}>
            <ABSColorPalette color="#ffffff" />
          </Col>
          <Col span={4}>
            <ABSColorPalette color="#ffffff" />
          </Col>
          <Col span={4}>
            <ABSColorPalette color="#ffffff" />
          </Col>
        </Row>
      </div>
    );
  }
}
 
export default ABSUIColor;