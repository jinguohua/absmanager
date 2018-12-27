import React from 'react';
import classNames from 'classnames';
import './absprogressItem.less';

export interface IABSProgressItemLineProps {
  inactiveRatio: number;
  hide?: boolean;
}
 
export interface IABSProgressItemLineState {
  
}
 
class ABSProgressItemLine extends React.Component<IABSProgressItemLineProps, IABSProgressItemLineState> {
  render() { 
    const { inactiveRatio, hide } = this.props;
    const backgroundStyle = classNames('progress-item-left-line', hide ? 'progress-item-left-line-transparent' : '');
    const lineStyle = classNames('progress-item-left-line-inactive', hide ? 'progress-item-left-line-transparent' : '');
    return (
      <div className={backgroundStyle}>
        <div className={lineStyle} style={{ flexGrow: inactiveRatio }} />
      </div>
    );
  }
}
 
export default ABSProgressItemLine;