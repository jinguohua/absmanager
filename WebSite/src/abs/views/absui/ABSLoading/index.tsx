import React, { Component } from 'react';
import { Row, Col } from 'antd';
import ABSLoading from '../../../../components/ABSLoading';
import ABSDescription from '../../../../components/ABSDescription';

class ABSLoadingRoute extends Component {
  render() {
    return (
      <div>
        <ABSDescription>loading加载图标</ABSDescription>
        <Row style={{ marginBottom: 15}} type="flex" align="middle">
          <Col span={1}>
            <ABSLoading size="small" />
          </Col>
          
          <Col span={1}>
            <ABSLoading size="small" color="blue" />
          </Col>
          
          <Col span={10}>
            白色：放在蓝色按钮上；小的用于容器局部加载
          </Col>
        </Row>
        <Row style={{ marginBottom: 15}} type="flex" align="middle">
          <Col span={1}>
            <ABSLoading />
          </Col>
          
          <Col span={1}>
            <ABSLoading color="blue" />
          </Col>
          
          <Col span={10}>
            中的视情况而定，一般用于块区域加载
          </Col>
        </Row>
        <Row type="flex" align="middle">
          <Col span={1}>
            <ABSLoading size="large" />
          </Col>
          
          <Col span={1}>
            <ABSLoading size="large" color="blue"/>
          </Col>
          
          <Col span={10}>
            大的用于页面全局加载，一般用于整个页面刷新。
          </Col>
        </Row>
      </div>
    );
  }
}

export default ABSLoadingRoute;