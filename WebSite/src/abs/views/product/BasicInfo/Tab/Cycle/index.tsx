import React from 'react';
import ABSProgress from '../../../../../../components/ABSProgress';
import { IProgressNode } from '../../../../../../components/ABSProgress/ABSProgressItem';
import './index.less';
import PerfectScrollbar from 'react-perfect-scrollbar';

export interface ICycleProps {
  cycles: IProgressNode[];
}

export interface ICycleState {

}

class Cycle extends React.Component<ICycleProps, ICycleState> {
  render() {
    const { cycles } = this.props;
    return (
      <div className="product-cycle">
        <PerfectScrollbar>
          <ABSProgress dataSource={cycles} className="product-cycle-progress" />
        </PerfectScrollbar>
      </div>
    );
  }
}

export default Cycle;