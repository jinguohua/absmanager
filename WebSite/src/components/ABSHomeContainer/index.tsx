import React from 'react';
import './index.less';
import classnames from 'classnames';

export interface IABSHomeContainerProps {
  children?: any;
  style?: any;
  className?: string;
}

class ABSHomeContainer extends React.Component<IABSHomeContainerProps> {
  render() {
    const { children, className, style } = this.props;
    const clazs = classnames('abs-home-container', className);
    return (
      <div className="abs-home">
        <div className={clazs} style={style}>
          {children}
        </div>
      </div>
    );
  }
}

export default ABSHomeContainer;