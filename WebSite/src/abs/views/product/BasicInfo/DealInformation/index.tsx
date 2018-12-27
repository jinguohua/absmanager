import React, { Component } from 'react';
import { connect } from 'dva';
import './index.less';
import DealTable from './DealTable';
import DealImages from './DealImages';
import DealInfo from './DealInfo';

export interface IDealInformationProps {
  dispatch: ({ }: any) => void;
  detail: any;
  structureChart: any;
  paymentChart: any;
  dealID: number;
}

export interface IDealInformationState {
}

class DealInformation extends Component<IDealInformationProps, IDealInformationState> {

  render() {
    return (
      <div className="deal-infomation">
        <DealInfo />
        <DealImages />
        <DealTable />
      </div>
    );
  }
}

const mapStateToProps = ({ product }) => {
  return {};
};

export default connect(mapStateToProps)(DealInformation);