import React from 'react';
import './index.less';
import classnames from 'classnames';
import { Checkbox } from 'antd';

export interface IABSCheckboxProps {
  style?: React.CSSProperties;
  className?: string;
  content: string;
  value: string;
  disabled?: boolean;
  checked?: boolean;
  onChange?: (e: any) => void;
}

class ABSCheckbox extends React.Component<IABSCheckboxProps> {
  render() {
    const { className, style, content, value, disabled, checked, onChange } = this.props;
    const classs = classnames('abs-checkbox', className);
    if (checked) {
      return (
        <div className={classs}>
          <Checkbox style={style} value={value} disabled={disabled} checked={checked} onChange={onChange}>
            <span className="abs-checkbox-text">{content}</span>
          </Checkbox>
        </div>
      );
    }
    return (
      <div className={classs}>
        <Checkbox style={style} value={value} disabled={disabled} onChange={onChange}>
          <span className="abs-checkbox-text">{content}</span>
        </Checkbox>
      </div>
    );
  }
}

export default ABSCheckbox;