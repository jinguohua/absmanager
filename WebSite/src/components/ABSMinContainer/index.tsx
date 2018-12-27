import classNames from 'classnames';
import React from 'react';
import './index.less';

export interface IABSMinContainerProps {
  style?: React.CSSProperties;
  className?: string;
}
 
class ABSMinContainer extends React.Component<IABSMinContainerProps> {
  render() { 
    const { children, className } = this.props;
    const classes = classNames('min-container', className);
    return <div className={classes}>{children}</div>;
  }
}
 
export default ABSMinContainer;