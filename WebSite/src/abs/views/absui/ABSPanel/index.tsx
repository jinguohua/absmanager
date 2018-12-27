import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSPanel from '../../../../components/ABSPanel';
import './index.less';

class ABSUIPanel extends Component {
  render () {
    return (
      <div className="abs-panels">
        <ABSPanel title="默认带有内边距的panel" style={{ marginBottom: 30 }}>
          内部组件内容
        </ABSPanel>
        <ABSPanel title="去除内边距的panel" removePadding={true} style={{ marginBottom: 30 }}>
          内部组件内容
        </ABSPanel>
        <ABSPanel title="带红色注释" comment="注释注释注释" style={{ marginBottom: 30 }}>
          内部组件内容
        </ABSPanel>
        <ABSPanel title="带红色注释ReactNode" comment={<span className="abs-hasStar">注释注释注释</span>}>
          内部组件内容
        </ABSPanel>

        <ABSPanel title="内部正在加载" loading={true}>
          内部组件内容
        </ABSPanel>
      </div>
      
    );
  }
}

export default ABSUIPanel;