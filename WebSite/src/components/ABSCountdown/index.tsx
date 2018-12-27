import React from 'react';
import './index.less';
import classNames from 'classnames';

export interface IABSCountdownProps {
  className?: string;
  time?: number;
  onStartCountdown: () => Promise<boolean>;
}
 
export interface IABSCountdownState {
  count: number;
}

const DEFAULT_TIME = 60; // second
 
class ABSCountdown extends React.Component<IABSCountdownProps, IABSCountdownState> {
  timer: any;
  wait: boolean = false;

  constructor(props: IABSCountdownProps) {
    super(props);
    const { time } = props;
    this.state = {
      count: time ? time : DEFAULT_TIME,
    };
  }

  componentWillUnmount() {
    clearInterval(this.timer);
  }

  onClick = async () => {
    if (this.wait) {
      return;
    }
    const { onStartCountdown } = this.props;
    const onStart = await onStartCountdown();
    if (!onStart) {
      return;
    }
    this.timer = setInterval(() => {
      const { count } = this.state;
      if (count <= 1) {
        clearInterval(this.timer);
        this.wait = false;
        this.setState({ count: DEFAULT_TIME });
      } else {
        this.wait = true;
        this.setState({ count: count - 1 });
      }
    }, 1000);
  }

  render() {
    const { className } = this.props;
    const containerStyle = classNames('abs-countdown', { 'abs-countdown-wait': this.wait }, className);
    return (
      <div className={containerStyle} onClick={this.onClick}>
        {this.renderContent()}
      </div>
    );
  }

  renderContent() {
    const { count } = this.state;
    if (this.wait) {
      return (
        <p className="abs-countdown-text">
          {`${count}秒后重发`}
        </p>
      );
    }
    return <p className="abs-countdown-text">获取验证码</p>;
  }
}
 
export default ABSCountdown;