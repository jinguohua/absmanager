import React from 'react';
import './index.less';
import { Radio } from 'antd';
import classnames from 'classnames';
export interface IABSRadioProps {
  style?: React.CSSProperties;
  className?: string;
  content: string;
  value: string;
  disabled?: boolean;
  checked?: boolean;
  onChange?: (e: any) => void;
}

class ABSRadio extends React.Component<IABSRadioProps> {
  render() {
    const { style, className, content, value, disabled, checked, onChange } = this.props;
    const classs = classnames('abs-radio', className);
    if (checked) {
      return (
        <div>
          <Radio style={style} value={value} disabled={disabled} checked={checked} className={classs} onChange={onChange}>
            <span className="abs-radio-text">{content}</span>
          </Radio>
        </div>
      );
    }
    return (
      <div>
        <Radio style={style} value={value} disabled={disabled} className={classs} onChange={onChange}>
          <span className="abs-radio-text">{content}</span>
        </Radio>
      </div>
    );
  }
}

export default ABSRadio;