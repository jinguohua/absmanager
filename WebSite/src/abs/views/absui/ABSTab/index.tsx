import React, { Component } from 'react';
import { ABSTab } from '../../../../components/ABSTab';
import ABSDescription from '../../../../components/ABSDescription';
import { Row } from 'antd';

class ABSTabRoute extends Component<any, any> {
  render() {
    const panesList1 = [
      {
        title: '相关专家',
        key: '1',
        content: '相关专家'
      },
      {
        title: '产品周期',
        key: '2',
        content: '产品周期'
      },
      {
        title: '参与机构',
        key: '3',
        content: '参与机构'
      }
    ];
    const panesList2 = [
      {
        title: '国内用户',
        key: '1',
        content: '国内用户'
      },
      {
        title: '国外用户',
        key: '2',
        content: '国外用户'
      }
    ];
    const activeKey = '1';
    return (
      <div style={{ width: '300px' }}>
        <ABSDescription>tab切换标签</ABSDescription>
        <Row style={{ marginBottom: 32 }}>
          <ABSTab activeKey={activeKey} panesList={panesList1} />{' '}
        </Row>

        <Row style={{ marginBottom: 12 }}>
          <ABSTab activeKey={activeKey} panesList={panesList2} type="full" />{' '}
        </Row>
        {/* 默认，滚条与文字内容等宽 */}
        
        {/* type=full时， 滚条与tab单元格等宽（单个tab撑满）*/}
      </div>
    );
  }
}

export default ABSTabRoute;
