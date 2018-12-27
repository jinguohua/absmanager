import React, { Component } from 'react';
import './index.less';
import ABSSubTitle from '../../../../components/ABSSubTitle';
import { ABSButton } from '../../../../components/ABSButton';

class ABSUISubTitle extends Component {

  renderRight() {
    return (
      <div className="right">
        <ABSButton icon="star">{'计算'}</ABSButton>
        <ABSButton icon="star">{'新增情景'}</ABSButton>
      </div>
    );
  }

  render() {
    return (
      <div>
        <ABSSubTitle title="结果显示" buttonTitle="计算" buttonIcon="star" />
        <ABSSubTitle title="结果显示" renderRight={this.renderRight} />
      </div>
    );
  }
}

export default ABSUISubTitle;