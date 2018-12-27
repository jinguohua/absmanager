import React from 'react';
import moment from 'moment';
import ABSRangeDatePicker from '../../../../components/ABSRangeDatePicker';

export interface IABSUIDoubleDatePickerProps {
  
}
 
export interface IABSUIDoubleDatePickerState {
  expectedBuildDateStart: moment.Moment;
  expectedBuildDateEnd: moment.Moment;
}
 
class ABSUIDoubleDatePicker extends React.Component<IABSUIDoubleDatePickerProps, IABSUIDoubleDatePickerState> {
  constructor(props: IABSUIDoubleDatePickerProps) {
    super(props);
    this.state = {
      expectedBuildDateStart: moment('2017-01-02', 'YYYY-MM-DD'),
      expectedBuildDateEnd: moment('2017-01-02', 'YYYY-MM-DD'),
    };
  }

  onExpectedBuildDateStartValueChange = (value: moment.Moment) => {
    this.setState({ expectedBuildDateStart: value });
  }

  onExpectedBuildDateEndValueChange = (value: moment.Moment) => {
    this.setState({ expectedBuildDateEnd: value });
  }

  render() { 
    const { expectedBuildDateStart, expectedBuildDateEnd } = this.state;
    return (
      <ABSRangeDatePicker
        title="在职时间" 
        startValue={expectedBuildDateStart}
        endValue={expectedBuildDateEnd}
        onStartDateChange={this.onExpectedBuildDateStartValueChange}
        onEndDateChange={this.onExpectedBuildDateEndValueChange}
        splitStyle={{ width: 14, margin: '0 3px'}}
        isRequired={true}
        size="large"
      />
    );
  }
}
 
export default ABSUIDoubleDatePicker;