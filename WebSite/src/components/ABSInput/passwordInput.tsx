import React from 'react';
import { Input } from 'antd';
import './index.less';
import classNames from 'classnames';
import { ABSAntIcon } from '../ABSAntIcon';

export declare type inputSize = 'large' | 'default' | 'small';

interface IProps {
  placeholder?: string;
  type?: string;
  disabled?: boolean;
  readOnly?: boolean;
  defaultValue?: string;
  id?: string;
  size?: inputSize;
  onChange?: any;
  prefix?: string | React.ReactNode;
  suffix?: string | React.ReactNode;
  className?: string;
  value?: string;
  addonAfter?: string | React.ReactNode;
  addonBefore?: string | React.ReactNode;
  style?: React.CSSProperties;
}

// 登录密码输入框
export default class PasswordInput extends React.Component<IProps, any> {
  constructor(props: any) {
    super(props);
    this.state = {
      typetext: 'password',
    };
  }

  mouseEnter = (value) => {
    this.setState({ typetext: 'default' });
  }

  mouseLeave = (value) => {
    this.setState({ typetext: 'password' });
  }

  public render() {
    const { typetext } = this.state;
    const { placeholder, disabled, readOnly, defaultValue, id, size, prefix, className, value, addonAfter, addonBefore, style } = this.props;
    const inputStyle = classNames('abs-input', className);
    return (
      <div className={inputStyle} style={style}>
        <Input
          prefix={prefix}
          placeholder={placeholder}
          type={typetext}
          disabled={disabled}
          readOnly={readOnly}
          defaultValue={defaultValue}
          id={id}
          size={size}
          onChange={this.onChange}
          suffix={<div onMouseEnter={this.mouseEnter} onMouseLeave={this.mouseLeave}><ABSAntIcon type="eye" style={{ color: '#979797' }} /></div>}
          value={value}
          addonBefore={addonBefore}
          addonAfter={addonAfter}
        />
      </div>
    );
  }
  onChange = (value: any) => {
    const { onChange } = this.props;
    if (onChange && value) {
      onChange(value.target.value);
    }
  }
}
