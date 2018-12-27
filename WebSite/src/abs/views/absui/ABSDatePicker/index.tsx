import React, { Component } from 'react';
import ABSDatePicker from '../../../../components/ABSDatePicker';
import '../../../../styles/coverAnt.less';
import './index.less';
import ABSDescription from '../../../../components/ABSDescription';

export interface IAppProps {
}

export interface IAppState {
}
class ABSDatePickerRoute extends Component<IAppProps, IAppState> {
  
  render () {
    return (
      <div className="absui-date-picker">
        <ABSDescription>日历组件</ABSDescription>
        <ABSDatePicker />
      </div>
    );
  }
}

export default ABSDatePickerRoute;