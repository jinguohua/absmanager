import React, { Component } from 'react';
import ABSPageTitle from '../../../../components/ABSPageTitle';
import { ABSButton } from '../../../../components/ABSButton';

class ABSUIPageTitle extends Component {

  renderRights() {
    return (
      <div style={{ display: 'flex' }}>
        <ABSButton icon="star" >{'按钮'}</ABSButton>
      </div>
    );
  }

  render() {
    return (
      <div>
        <ABSPageTitle title="左侧为空的title" />
        <div style={{ height: 20 }} />
        <ABSPageTitle title="左侧不为空的title" renderRight={this.renderRights()} />
        <div style={{ height: 20 }} />
        <ABSPageTitle title="左侧为空的title" subTitle="这是子标题" />
        <div style={{ height: 20 }} />
        <ABSPageTitle title="左侧不为空的title" subTitle="这是子标题" renderRight={this.renderRights()} />
      </div>
    );
  }
}

export default ABSUIPageTitle;