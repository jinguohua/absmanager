import React from 'react';
import { Input } from 'antd';
import './index.less';
import classNames from 'classnames';
import { ABSAntIcon } from '../../components/ABSAntIcon';

export declare type inputSize = 'large' | 'default' | 'small';

export interface IABSInputProps {
  style?: React.CSSProperties;
  className?: string;
  placeholder?: string;
  value?: string;
  defaultValue?: string;
  id?: string;
  type?: string;
  size?: inputSize;
  disabled?: boolean;
  readOnly?: boolean;
  autosize?: any;
  prefix?: string | React.ReactNode;
  onChange?: (value: string) => void;
  onGetValue?: (value: string) => void | string;
  onBlur?: (value: string) => void;
  issuffix?: boolean;
  addonAfter?: string | React.ReactNode;
  addonBefore?: string | React.ReactNode;
  deletBtn?: (value: any) => void;
  icon?: any;
  regular?: any;
  getvalue?: any;
  inputType?: any;
  errortext?: string;
  emptyClassName?: string;
  rows?: number;
}

interface IABSInputState {
  value: string;
  emptyClassName: string;
}

export class ABSInput extends React.Component<IABSInputProps, IABSInputState> {
  public static defaultProps = {
    issuffix: false,
  };

  constructor(props: any) {
    super(props);
    const { value } = this.props;
    this.state = {
      value: value ? value : '',
      emptyClassName: '',
    };
  }

  componentWillReceiveProps(nextProps: any) {
    if (nextProps.value !== this.props.value) {
      this.setState({ value: nextProps.value, emptyClassName: '' });
    }
    if (nextProps.emptyClassName !== this.props.emptyClassName) {
      this.setState({ emptyClassName: nextProps.emptyClassName });
    }
  }

  icon(value: any, regular: any) {
    const { deletBtn } = this.props;
    if (!value) {
      return null;
    }
    if (value && (regular.test(value))) {
      return <ABSAntIcon type={'check-circle'} onClick={deletBtn} style={{ color: 'green' }} theme="filled" />;
    }
    return <ABSAntIcon type={'close-circle'} onClick={deletBtn} style={{ color: '#979797' }} />;
  }

  iconButton = () => {
    const { issuffix, deletBtn, getvalue, inputType, regular } = this.props;
    if (!issuffix || JSON.stringify(getvalue) === '{}' || !getvalue) {
      return null;
    }
    switch (inputType) {
      case 'mobile':
        return this.icon(getvalue.mobile, regular);
      case 'email':
        return this.icon(getvalue.email, regular);
      case 'form':
        return this.icon(getvalue.form, regular);
      default: {
        return <ABSAntIcon type="close-circle" onClick={deletBtn} style={{ color: '#979797' }} />;
      }
    }
  }

  render() {
    const { placeholder, type, disabled, readOnly, defaultValue, id, size, prefix, className, addonAfter, addonBefore, style, autosize, rows } = this.props;
    const { value, emptyClassName } = this.state;
    const inputStyle = classNames('abs-input', className);
    const emptyStyle = emptyClassName ? classNames('abs-input-empty', emptyClassName) : '';
    const suffix = this.iconButton();
    if (autosize) {
      return (
        <div className={inputStyle}>
          <Input.TextArea
            placeholder={placeholder}
            disabled={disabled}
            readOnly={readOnly}
            defaultValue={defaultValue}
            id={id}
            onChange={this.onChange}
            value={value}
            style={style}
            rows={rows}
          />
        </div>
      );
    }
    return (
      <div className={inputStyle} style={style}>
        <Input
          prefix={prefix}
          placeholder={placeholder}
          type={type}
          disabled={disabled}
          readOnly={readOnly}
          defaultValue={defaultValue}
          id={id}
          size={size}
          onChange={this.onChange}
          onBlur={this.onBlur}
          suffix={suffix}
          value={value}
          addonBefore={addonBefore}
          addonAfter={addonAfter}
          className={emptyStyle}
          style={style}
        />
      </div>
    );
  }

  onChange = (e: any) => {
    const { onChange } = this.props;
    if (onChange) {
      onChange(e.target.value);
      return;
    }
    this.setState({ value: e.target.value });
  }

  onBlur = (e: any) => {
    const { onGetValue, onBlur } = this.props;
    if (onGetValue) {
      const flag = onGetValue(e.target.value);
      if (typeof flag === 'undefined') {
        return;
      }
      this.setState({ emptyClassName: flag });
    }
    if (onBlur) {
      onBlur(e.target.value);
    }
  }
}
