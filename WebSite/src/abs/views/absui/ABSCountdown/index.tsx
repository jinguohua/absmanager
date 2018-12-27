import React from 'react';
import ABSCountdown from '../../../../components/ABSCountdown';
import ABSDescription from '../../../../components/ABSDescription';

export interface IABSCountdownSampleProps {
  
}
 
export interface IABSCountdownSampleState {
  
}
 
class ABSCountdownSample extends React.Component<IABSCountdownSampleProps, IABSCountdownSampleState> {
  onStartCountdown = async () => {
    return true;
  }

  render() { 
    return (
      <div className="absui-countdown">
        <ABSDescription>倒计时组件：用于发送验证码倒计时</ABSDescription>
        <ABSCountdown onStartCountdown={this.onStartCountdown} />
      </div>
    );
  }
}
 
export default ABSCountdownSample;