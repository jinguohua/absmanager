import React from 'react';
import Label from '../Label';
import { ABSInput } from '../../ABSInput';
import './index.less';

export interface ISampleInputProps {
  title: string;
  onTextChange: (value: string) => void;
  inputValue: string;
  placeholder: string;
}
  
export interface ISampleInputState {
  
}
 
class SampleInput extends React.Component<ISampleInputProps, ISampleInputState> {
  render() { 
    const { title, onTextChange, inputValue, placeholder } = this.props;
    return (
      <div className="abs-single-input">
        <Label title={title} className="abs-single-input-title" />
        <ABSInput
          placeholder={placeholder}
          size="default"
          onChange={onTextChange}
          value={inputValue}
        />
      </div>
    );
  }
}
 
export default SampleInput;