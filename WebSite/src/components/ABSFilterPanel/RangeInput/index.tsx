import React from 'react';
import Label from '../Label';
import { ABSInput } from '../../ABSInput';
import './index.less';

export interface IRangeInputProps {
  title: string;
  startValue: string;
  endValue: string;
  onStartValueChange: (value: string) => void;
  onEndValueChange: (value: string) => void;
  startFieldPlaceholder: string;
  endFieldPlaceholder: string;
}
 
export interface IRangeInputState {
  
}
 
class RangeInput extends React.Component<IRangeInputProps, IRangeInputState> {
  render() { 
    const { title, startValue, endValue, onStartValueChange, onEndValueChange, startFieldPlaceholder, endFieldPlaceholder } = this.props;
    return (
      <div className="abs-double-input">
        <Label title={title} className="abs-double-input-title" />
        <ABSInput
          placeholder={startFieldPlaceholder}
          size="default"
          onChange={onStartValueChange}
          value={startValue}
        />
        <div style={{ width: '8px', height: '2px', margin: '0 5px', verticalAlign: 'middle', display: 'inline-block', backgroundColor: '#999999' }}> {/**/} </div>
        <ABSInput
          placeholder={endFieldPlaceholder}
          size="default"
          onChange={onEndValueChange}
          value={endValue}
        />
      </div>
    );
  }
}
 
export default RangeInput;