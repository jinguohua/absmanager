import React from 'react';
import { BackTop } from 'antd';
import ABSIcon from '../ABSIcon';
import './index.less';

export interface IABSBackTopProps {
  dom: HTMLElement;
}
 
class ABSBackTop extends React.Component<IABSBackTopProps> {
  getTarget = () => {
    const { dom } = this.props;
    return dom as HTMLElement;
  }

  render() { 
    return (
      <div className="abs-back-top">
        <BackTop target={this.getTarget} visibilityHeight={0}>
          <div className="ant-back-top-inner">
            <ABSIcon type="prev-one" />
          </div>
        </BackTop>
      </div>
    );
  }
}
 
export default ABSBackTop;