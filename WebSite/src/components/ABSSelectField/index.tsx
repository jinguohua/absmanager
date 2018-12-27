import React from 'react';
import './index.less';
import Title from '../ABSSpan';
import { ABSSelect } from '../ABSSelect';

export interface ISelectFieldProps {
  title: string;
  placeholder?: string;
  defaultValue?: any;
  selectOptions: any[];
  className?: string;
  required?: boolean;
}
 
export interface ISelectFieldState {
  value: string;
}
 
class ABSSelectField extends React.Component<ISelectFieldProps, ISelectFieldState> {
  constructor(props: ISelectFieldProps) {
    super(props);
    const { defaultValue } = props;
    this.state = {
      value: defaultValue ? defaultValue : '',
    };
  }

  selectChange = (value) => {
    this.setState({ value });
  }

  selectSelect = () => { 
    // 被选中时调用
  }

  selectSearch = () => { 
    // 文本框值变化时回调
  }
  
  selectFocus = () => { 
    // 获得焦点时回调
  }

  selectBlur = () => { 
    // 失去焦点的时回调 
  }

  getValue() {
    return this.state.value;
  }

  getIntValue() {
    const intValue = parseInt(this.state.value, 10);
    if (isNaN(intValue)) { return 0; }
    return intValue;
  }

  render() {
    const { 
      title, 
      placeholder, 
      selectOptions,
      className,
      required,
    } = this.props;
    const { value } = this.state;
    return (
      <div className={`select-field ${className}`}>
        <Title className="select-field-title" required={required}>{title}</Title>
        <ABSSelect 
          placeholder={placeholder} 
          className=""  
          disabled={false} 
          size="default" 
          onChange={this.selectChange} 
          onSelect={this.selectSelect} 
          onSearch={this.selectSearch} 
          onFocus={this.selectFocus} 
          onBlur={this.selectBlur}
          dropdownMatchSelectWidth={true} 
          dropdownClassName=""
          optionData={selectOptions}
          value={value}
        />
      </div>
    );
  }
}
 
export default ABSSelectField;