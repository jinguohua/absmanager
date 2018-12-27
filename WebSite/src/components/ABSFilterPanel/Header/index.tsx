import React from 'react';
import { FILTER_DEFAULT_START_DATE } from '../../../utils/constant';
import ABSRangeDatePicker from '../../ABSRangeDatePicker';
import moment from 'moment';

export interface IHeaderProps {
  
}
 
export interface IHeaderState {
  startDate: moment.Moment;
  endDate: moment.Moment;
}

const DATE_FORMAT = 'YYYY-MM-DD';
 
class Header extends React.Component<IHeaderProps, IHeaderState> {
  state = {
    startDate: moment(FILTER_DEFAULT_START_DATE, DATE_FORMAT),
    endDate: moment(),
  };

  onStartDateChange = (value: moment.Moment) => {
    this.setState({ startDate: value });
  }

  onEndDateChange = (value: moment.Moment) => {
    this.setState({ endDate: value });
  }

  extraValue() {
    const { startDate, endDate } = this.state;
    const startDateString = startDate.format(DATE_FORMAT);
    const endDateString = endDate.format(DATE_FORMAT);
    return {
      statistics_date: {
        start: startDateString,
        end: endDateString,
      },
    };
  }

  render() { 
    const { startDate, endDate } = this.state;
    return (
      <ABSRangeDatePicker 
        title="统计日期" 
        startValue={startDate}
        endValue={endDate}
        onStartDateChange={this.onStartDateChange}
        onEndDateChange={this.onEndDateChange}
      />
    );
  }
}
 
export default Header;