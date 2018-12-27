import React, { Component } from 'react';
import { ABSAlert } from '../../../../components/ABSAlert';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';
import { Row } from 'antd';
import { ABSUI_MARGIN_BOTTOM } from '../../../../utils/constant';

// 提示演示
class ABSAlertRoute extends Component {
  render() {
    return (
      <div>
        <ABSDescription>页头提示（一般应用于页面浮层提示，通过交叉关闭后隐藏）</ABSDescription>
        <Row style={{ marginBottom: ABSUI_MARGIN_BOTTOM }}>
          <ABSAlert message="恭喜！你所提交的信息已审核通过，请及时跟进申请状况。如有疑问，请联系客服。" type="success" />
        </Row>
        <Row style={{ marginBottom: ABSUI_MARGIN_BOTTOM}}>
          <ABSAlert message="请输入您想完善的数据详情，也可以直接拨打电话：021-31156258" type="info" />
        </Row>
        <Row style={{ marginBottom: ABSUI_MARGIN_BOTTOM}}>
          <ABSAlert message="系统将于今日15:00-17:00进行升级，请及时保存你的资料！" type="warning" />
        </Row>
        <Row style={{ marginBottom: ABSUI_MARGIN_BOTTOM}}>
          <ABSAlert message="系统错误，请稍后重试。" type="error" />
        </Row>
      </div>
    );
  }
}

export default ABSAlertRoute;