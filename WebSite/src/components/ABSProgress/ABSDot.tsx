import React from 'react';
import './absprogressItem.less';
import classNames from 'classnames';

export interface IABSDotProps {
  active: boolean;
}
 
export interface IABSDotState {
  
}
 
class ABSDot extends React.Component<IABSDotProps, IABSDotState> {
  render() { 
    const { active } = this.props;
    const dotStyle = classNames('progress-item-left-dot', active ? 'progress-item-left-dot-active' : 'progress-item-left-dot-inactive');
    return <div className={dotStyle} />;
  }
}
 
export default ABSDot;