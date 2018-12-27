import React from 'react';
import './index.less';
import { ABSInput } from '../ABSInput';
import ABSMessage from '../ABSMessage';
import Span from '../ABSSpan';

export interface IInputFieldProps {
  title: string;
  placeholder?: string;
  className?: string;
  defaultValue?: string | null;
  maxLength: number;
  required?: boolean;
  autosize?: boolean;
  rows?: number;
  style?: any;
  disabled?: boolean;
}

export interface IInputFieldState {
  inputValue: string;
}

class ABSInputField extends React.Component<IInputFieldProps, IInputFieldState> {
  constructor(props: IInputFieldProps) {
    super(props);
    const { defaultValue } = props;
    this.state = {
      inputValue: defaultValue ? defaultValue : '',
    };
  }

  componentWillReceiveProps(nextProps: IInputFieldProps) {
    const { defaultValue } = nextProps;
    this.setState({ inputValue: defaultValue ? defaultValue : '' });
  }

  onChange = (value: string) => {
    const { maxLength } = this.props;
    if (value.length > maxLength) {
      ABSMessage.error(`输入字符超过了允许的最大长度（${maxLength}）`);
      return;
    }
    this.setState({ inputValue: value });
  }

  getValue() {
    return this.state.inputValue;
  }

  getIntValue() {
    let intValue = parseInt(this.state.inputValue, 10);
    if (isNaN(intValue)) { intValue = 0; }
    return intValue;
  }

  render() {
    const {
      title,
      placeholder,
      className,
      required,
      rows,
      style,
      autosize,
      disabled,
    } = this.props;
    const { inputValue } = this.state;
    return (
      <div className={`input-field ${className}`}>
        <Span required={required} className="input-field-title">{title}</Span>
        <ABSInput
          onChange={this.onChange}
          value={inputValue}
          placeholder={placeholder}
          className="input-field-input"
          autosize={autosize}
          rows={rows}
          style={style}
          disabled={disabled ? disabled : false}
        />
      </div>
    );
  }
}

export default ABSInputField;