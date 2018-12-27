import React, { Component } from 'react';
import ABSComment from '../../../../components/ABSComment';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';

// 提示演示
class ABSCommentRoute extends Component {
  render() {
    return (
      <div style={{ padding: 20}}>
        <ABSDescription>注释组件(通常用于ABSPanel组件中)</ABSDescription>
        <ABSComment>*参考利率数据来源于同期同评级短融中票</ABSComment>
      </div>
    );
  }
}

export default ABSCommentRoute;