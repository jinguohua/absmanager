import React from 'react';
import './index.less';
import ABSDatePicker from '../ABSDatePicker';
import classNames from 'classnames';
import moment from 'moment';
import Label from '../ABSFilterPanel/Label';

export interface IABSDoubleDatePickerProps {
  title?: string | React.ReactNode;
  startValue?: moment.Moment;
  endValue?: moment.Moment;
  onStartDateChange: (date: moment.Moment | string) => void;
  onEndDateChange: (date: moment.Moment | string) => void;

  disabledDate?: (currentDate: moment.Moment) => boolean;
  splitStyle?: React.CSSProperties;
  isRequired?: boolean;
  size?: 'small' | 'large';
  // 隐藏头部
  hideTitle?: boolean;
}
 
class ABSDoubleDatePicker extends React.Component<IABSDoubleDatePickerProps> {
  static defaultProps() {
    return {
      isRequired: false,
      hideTitle: false,
    };
  }

  disabledStartDate = (startValue) => {
    const { endValue } = this.props;
   
    if (!startValue || !endValue) {
      return false;
    }

    return startValue.valueOf() > endValue.valueOf();
  }

  disabledEndDate = (endValue) => {
    const { startValue } = this.props;

    if (!endValue || !startValue) {
      return false;
    }

    return endValue.valueOf() <= startValue.valueOf();
  }

  renderTitle() {
    const { title, isRequired } = this.props;
    if (isRequired) {
      return <span><span className="abs-double-date-picker-label-required">*</span>{title}</span>;
    }
    return title;
  }

  showTitle() {
    const { hideTitle } = this.props;
    if (!hideTitle) {
      return <Label title={this.renderTitle()} className="abs-double-input-title" />;
    }
    return null;
  }

  render() {
    const { splitStyle, size, onStartDateChange, onEndDateChange, startValue, endValue } = this.props;
    const classes = classNames('abs-double-date-picker', {
      'abs-double-date-picker-large': size === 'large'
    });
    const dropdownClasses = classNames('abs-double-date-picker-dropdown', {
      'abs-double-date-picker-dropdown-large': size === 'large'
    });
    return (
      <div className={classes}>
        {this.showTitle()}
        
        <ABSDatePicker 
          className="abs-double-date-picker-component"
          disabledDate={this.disabledStartDate}
          onChange={onStartDateChange} 
          format="YYYY-MM-DD"
          dropdownClassName={dropdownClasses}
          value={startValue}
          placeholder={'起'}
        />
        <div style={{ width: '8px', height: '2px', margin: '0 5px', verticalAlign: 'middle', display: 'inline-block', backgroundColor: '#999999', ...splitStyle }} />
        <ABSDatePicker 
          className="abs-double-date-picker-component"
          disabledDate={this.disabledEndDate}
          onChange={onEndDateChange} 
          format="YYYY-MM-DD"
          dropdownClassName={dropdownClasses}
          value={endValue}
          placeholder={'止'}
        />
      </div>
    );
  }
}
 
export default ABSDoubleDatePicker;