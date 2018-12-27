import React from 'react';
import classNames from 'classnames';
import PerfectScrollbar from 'react-perfect-scrollbar';
import './index.less';

export interface IABSContainerProps {
  children: React.ReactNode;
  style?: React.CSSProperties;
  className?: string;
  removePerfectScrollBar?: boolean;
  removePadding?: boolean;
}
 
class ABSContainer extends React.Component<IABSContainerProps> {

  render() { 
    const { children, className, style, removePerfectScrollBar, removePadding } = this.props;
    const classes = classNames('abs-container', className, {
      'abs-container-no-padding': removePadding,
    });

    if (removePerfectScrollBar) {
      return (
        <div className={classes} style={style}>
          {children}
        </div>
      );
    }
    return (
      <div className={classes} style={style}>
        <PerfectScrollbar>
          {children}
        </PerfectScrollbar>
      </div>
    );
  }
}
 
export default ABSContainer;