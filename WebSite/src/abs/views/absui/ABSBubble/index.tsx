import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import { Tooltip } from 'antd';
import '../../../../components/ABSBubble/index.less';
import ABSDescription from '../../../../components/ABSDescription';
import './index.less';

export interface IAppProps {
}

export interface IAppState {
}
class ABSBubbleRoute extends Component<IAppProps, IAppState> {

  render() {
    return (
      <div className="absui-bubble">
        <ABSDescription style={{ marginBottom: 32}}>悬浮文本显示气泡</ABSDescription>
        <Tooltip title="12334" placement="top" arrowPointAtCenter={true}>
          <span>这是一个气泡文件示例</span>
        </Tooltip>
      </div>
      
    );
  }
}

export default ABSBubbleRoute;