import React from 'react';
import { Icon } from 'antd';
import './index.less';

export interface IABSIconProps {
  type: string;
  theme?: 'filled' | 'outlined' | 'twoTone';
  className?: string;
  style?: React.CSSProperties;
  spin?: boolean; // 是否有旋转动画
  onClick?: (e: any) => void;
}

export class ABSAntIcon extends React.Component<IABSIconProps> {
  static defaultProps = {
    type: 'check',
    theme: 'outlined',
    className: 'abs-ant-icon-s',
    spin: false,
  };
  
  render() {
    const { 
      className, 
      type, 
      theme, 
      style, 
      spin,
      onClick,
    } = this.props;

    // 判断icon是否有白底
    const iconBgShow = { 
      display: ((type.indexOf('-circle') > -1 && theme === 'filled') ? 'inline-block' : 'none') 
    };

    return (
      <div className="abs-ant-icon">
        <Icon
          type={type}
          className={className}
          theme={theme} 
          spin={spin}
          style={{ ...style }}
          onClick={onClick}
      />
        <span className="abs-ant-icon-bg" style={iconBgShow} />
      </div>
    );
  }
}