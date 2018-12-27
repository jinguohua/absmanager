import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import { ABSUploadCard } from '../../../../components/ABSUploadCard';
import ABSDescription from '../../../../components/ABSDescription';

export interface IAppProps {
}

export interface IAppState {

}

class ABSUploadRoute extends Component<IAppProps, IAppState> {
  render() {
    return (
      <div style={{ padding: 20}}>
        <ABSDescription>上传图片组件</ABSDescription>
        <ABSUploadCard/>
      </div>
    );
  }
}

export default ABSUploadRoute;