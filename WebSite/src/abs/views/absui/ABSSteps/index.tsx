import React from 'react';
import ABSSteps from '../../../../components/ABSSteps';
import './index.less';
import { ABSButton } from '../../../../components/ABSButton';

const ExampleComponent = ({ content }) => {
  return (
    <div className="step-content-sample">
      {content}
    </div>
  );
};

export const steps = [{
  title: '验证手机号',
  component: () => <ExampleComponent content="内容 1" />,
}, {
  title: '账号信息',
  component: () => <ExampleComponent content="内容 2" />,
}, {
  title: '用户信息',
  component: () => <ExampleComponent content="内容 3" />,
}];

export interface IABSStepsSampleProps {
  
}
 
export interface IABSStepsSampleState {
  current: number;
}
 
class ABSStepsSample extends React.Component<IABSStepsSampleProps, IABSStepsSampleState> {
  step: ABSSteps | null;

  onNext = () => {
    if (this.step) {
      // const current = this.step.next();
    }
  }

  onPrev = () => {
    if (this.step) {
      // const current = this.step.prev();
    }
  }

  render() {
    return (
      <div className="abs-steps-sample">
        <ABSSteps ref={step => this.step = step} initial={0} steps={steps} />
        <div className="abs-steps-sample-btn">
          <ABSButton onClick={this.onPrev} style={{ marginRight: 8 }}>上一步</ABSButton>
          <ABSButton onClick={this.onNext}>下一步</ABSButton>
        </div>
      </div>
    );
  }
}
 
export default ABSStepsSample;