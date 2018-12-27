import React from 'react';
import './index.less';
import { DatePicker, Icon } from 'antd';
import locale from 'antd/lib/date-picker/locale/zh_CN';
import classNames from 'classnames';
import moment from 'moment';
import 'moment/locale/zh-cn';
moment.locale('zh-cn');

export interface IDatePickerProps {
  className?: string;
  onChange?: (date: moment.Moment) => void;
  defaultValue?: moment.Moment;
  open?: boolean;
  onOpenChange?: (status: boolean) => void;
  disabledDate?: (currentDate: moment.Moment) => boolean;
  value?: moment.Moment;
  format?: string;
  dropdownClassName?: string;
  placeholder?: string;
}

export default class ABSDatePicker extends React.Component<IDatePickerProps, {}> {

  render() {
    const { className, defaultValue, onOpenChange, disabledDate, format, onChange, dropdownClassName, placeholder } = this.props;
    const classes = classNames('abs-calendar', className);

    return (
      <div className={classes}>
        <DatePicker
          dateRender={(current) => {
            let date = current.date().toString();
            if (current.dayOfYear() === moment().dayOfYear()) {
              date = '今';
            }
            return (
              <div className="ant-calendar-date">
                {date}
              </div>
            );
          }}
          defaultValue={defaultValue}
          placeholder={placeholder ? placeholder : '请选择日期'}
          showToday={false}
          locale={locale}
          className={className}
          onChange={onChange}
          allowClear={false}
          suffixIcon={<Icon type="down" />}
          onOpenChange={onOpenChange}
          disabledDate={disabledDate}
          format={format}
          dropdownClassName={dropdownClassName}
          {...this.props}
        />
      </div>
    );
  }
}
