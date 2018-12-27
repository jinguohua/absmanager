import React from 'react';
import './index.less';

export interface ILabelProps {
  title: string | React.ReactNode;
  className?: string;
}
 
export interface ILabelState {
  
}
 
class Label extends React.PureComponent<ILabelProps, ILabelState> {
  render() { 
    const { title, className } = this.props;
    return (
      <div className={`abs-filter-label ${className}`}>{title}</div>
    );
  }
}
 
export default Label;