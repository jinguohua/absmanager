import React from 'react';
import { Spin } from 'antd';
import { ABSAntIcon } from '../ABSAntIcon';
import './index.less';

export interface IABSLoadingProps {
    size?: 'small' | 'default' | 'large';
    color?: 'blue' | 'white';
    className?: string;
}

class ABSLoading extends React.Component<IABSLoadingProps> {
  public static defaultProps = {
    size: 'default',
    color: 'white',
  };

  render() {
    const { size, color, className } = this.props;
    const iconClass = `abs-loading-${size} abs-loading-${color} ${className}`;
    const icon = <ABSAntIcon type="loading" className={iconClass} />;
    return(
      <div className="abs-loading">
        <Spin indicator={icon} className={`abs-loading-${size}`} />
      </div>
    );
  }
}

export default ABSLoading;