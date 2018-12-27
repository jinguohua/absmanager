import React from 'react';
import { Button } from 'antd';
import classNames from 'classnames';
import './index.less';
declare type ButtonHTMLType = 'submit' | 'button' | 'reset';

export interface IbtnProps {
  className?: string;
  type?: 'primary' | 'default' | 'danger';
  style?: React.CSSProperties;
  icon?: string;
  children?: React.ReactNode;
  disabled?: boolean;
  block?: boolean;
  large?: boolean;
  htmlType?: ButtonHTMLType;
  onClick?: any; // 改为 onClick
}

export class ABSButton extends React.Component<IbtnProps, any> {
  static defaultProps = {
    type: 'primary',
    large: false,
    block: false,
  };

  render() {
    const {
      children,
      className,
      onClick,
      type,
      icon,
      block,
      style,
      disabled,
      large,
      htmlType
    } = this.props;
    const classes = classNames('abs-btn', className, {
      'abs-btn-large': large,
      'abs-btn-block': block,
    });
    const displayStyle = block ? 'block' : 'inline-block';

    return (
      <div className={classes} style={{ display: displayStyle, ...style }}>
        <Button
          icon={icon}
          onClick={onClick}
          disabled={disabled}
          type={type}
          block={block}
          htmlType={htmlType}
        >
          {children}
        </Button>
      </div>
    );
  }
}
