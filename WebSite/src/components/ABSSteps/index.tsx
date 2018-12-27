import React from 'react';
import { Steps } from 'antd';
import { IStepOption } from './interface';
import './index.less';
import { getStepContent } from './util';

const Step = Steps.Step;

export interface IABSStepsProps {
  initial: number;
  steps: IStepOption[];
  className?: string;
}
 
export interface IABSStepsState {
  current: number;
}
 
class ABSSteps extends React.Component<IABSStepsProps, IABSStepsState> {
  constructor(props: IABSStepsProps) {
    super(props);
    this.state = {
      current: props.initial
    };
  }

  next = () => {
    const { steps } = this.props;
    const { current } = this.state;
    const length = Array.isArray(steps) ? steps.length : 0;
    let stepIndex = current;
    if ((current + 1) >= length) {
      return stepIndex;
    }
    stepIndex = (current + 1) % length;
    this.setState({ current: stepIndex });
    return stepIndex;
  }

  prev = () => {
    const { steps } = this.props;
    const { current } = this.state;
    const length = Array.isArray(steps) ? steps.length : 0;
    let stepIndex = current;
    if (current <= 0) {
      return stepIndex;
    }
    stepIndex = (current - 1) % length;
    this.setState({ current: stepIndex });
    return stepIndex;
  }

  render() { 
    const { steps, className } = this.props;
    const { current } = this.state;
    return (
      <div className={className}>
        <Steps
          current={current} 
          labelPlacement="vertical"
        >
          {steps.map(item => <Step key={item.title} title={item.title} />)}
        </Steps>
        {this.renderContent()}
      </div>
    );
  }

  renderContent() {
    const { steps } = this.props;
    const { current } = this.state;
    const component = getStepContent(steps, current);
    return (
      <div>
        {component ? component() : null}
      </div>
    );
  }
}
 
export default ABSSteps;