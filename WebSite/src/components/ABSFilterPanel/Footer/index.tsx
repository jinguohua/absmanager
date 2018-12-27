import React from 'react';
import { floatValue } from './util';
import ABSRangeDatePicker from '../../ABSRangeDatePicker';
import moment from 'moment';
import RangeInput from '../../ABSFilterPanel/RangeInput';
import SampleInput from '../../ABSFilterPanel/SampleInput';

export interface IFooterProps {

}

export interface IFooterState {
  expectedBuildDateStart: moment.Moment | undefined; // 预成立日/有效期
  expectedBuildDateEnd: moment.Moment | undefined;
  durationStart: string;  // 期限(年)
  durationEnd: string;
  amountStart: string; // 金额(亿)
  amountEnd: string;
  rateStart: string; // 利率(%)
  rateEnd: string;
  shortName: string; // 简称筛选
}

class Footer extends React.Component<IFooterProps, IFooterState> {
  constructor(props: IFooterProps) {
    super(props);
    this.state = {
      expectedBuildDateStart: undefined,
      expectedBuildDateEnd: undefined,
      durationStart: '0',
      durationEnd: '30',
      amountStart: '0',
      amountEnd: '100',
      rateStart: '0',
      rateEnd: '30',
      shortName: '',
    };
  }

  resetBuildDate = () => {
    this.setState({
      expectedBuildDateStart: undefined,
      expectedBuildDateEnd: undefined,
      durationStart: '0',
      durationEnd: '30',
      amountStart: '0',
      amountEnd: '100',
      rateStart: '0',
      rateEnd: '30',
      shortName: '',
    });
  }

  onShortNameChange = (value: string) => {
    this.setState({ shortName: value });
  }

  onExpectedBuildDateStartValueChange = (value: moment.Moment) => {
    this.setState({ expectedBuildDateStart: value });
  }

  onExpectedBuildDateEndValueChange = (value: moment.Moment) => {
    this.setState({ expectedBuildDateEnd: value });
  }

  onDurationStartValueChange = (value: string) => {
    this.setState({ durationStart: value });
  }

  onDurationEndValueChange = (value: string) => {
    this.setState({ durationEnd: value });
  }

  onAmountStartValueChange = (value: string) => {
    this.setState({ amountStart: value });
  }

  onAmountEndValueChange = (value: string) => {
    this.setState({ amountEnd: value });
  }

  onRateStartValueChange = (value: string) => {
    this.setState({ rateStart: value });
  }

  onRateEndValueChange = (value: string) => {
    this.setState({ rateEnd: value });
  }

  extraValue() {
    const {
      expectedBuildDateStart,
      expectedBuildDateEnd,
      durationStart,
      durationEnd,
      amountStart,
      amountEnd,
      rateStart,
      rateEnd,
      shortName,
    } = this.state;
    const buildStartDate = expectedBuildDateStart ? expectedBuildDateStart.format('YYYY-MM-DD') : null;
    const buildEndDate = expectedBuildDateEnd ? expectedBuildDateEnd.format('YYYY-MM-DD') : null;
    return {
      expected_build_date: {
        start: buildStartDate,
        end: buildEndDate,
      },
      duration: {
        start: floatValue(durationStart),
        end: floatValue(durationEnd),
      },
      amount: {
        start: floatValue(amountStart),
        end: floatValue(amountEnd),
      },
      rate: {
        start: floatValue(rateStart),
        end: floatValue(rateEnd),
      },
      short_name: shortName,
    };
  }

  render() {
    const {
      expectedBuildDateStart,
      expectedBuildDateEnd,
      durationStart,
      durationEnd,
      amountStart,
      amountEnd,
      rateStart,
      rateEnd,
      shortName,
    } = this.state;
    return (
      <div>
        <ABSRangeDatePicker
          title="预成立日"
          startValue={expectedBuildDateStart}
          endValue={expectedBuildDateEnd}
          onStartDateChange={this.onExpectedBuildDateStartValueChange}
          onEndDateChange={this.onExpectedBuildDateEndValueChange}
        />
        <RangeInput
          title="期限(年)"
          startValue={`${durationStart}`}
          endValue={`${durationEnd}`}
          startFieldPlaceholder=""
          endFieldPlaceholder=""
          onStartValueChange={this.onDurationStartValueChange}
          onEndValueChange={this.onDurationEndValueChange}
        />
        <RangeInput
          title="金额(亿)"
          startValue={`${amountStart}`}
          endValue={`${amountEnd}`}
          startFieldPlaceholder=""
          endFieldPlaceholder=""
          onStartValueChange={this.onAmountStartValueChange}
          onEndValueChange={this.onAmountEndValueChange}
        />
        <RangeInput
          title="利率(%)"
          startValue={`${rateStart}`}
          endValue={`${rateEnd}`}
          startFieldPlaceholder=""
          endFieldPlaceholder=""
          onStartValueChange={this.onRateStartValueChange}
          onEndValueChange={this.onRateEndValueChange}
        />
        <SampleInput
          title="简称筛选"
          onTextChange={this.onShortNameChange}
          inputValue={shortName}
          placeholder="请输入关键字"
        />
      </div>
    );
  }
}

export default Footer;