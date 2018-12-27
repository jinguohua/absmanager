import React, { Component } from 'react';
import DrawResult from './DrawResult';
import ComputationResult from './ComputationResult';
import RelationAndResult from './relationAndResult';
import ABSContainer from '../../../../components/ABSContainer';
import ABSMinContainer from '../../../../components/ABSMinContainer';
import { connect } from 'dva';

export interface IBasicAnalysisProps {
  dispatch: any;
  dealID: number;
  scenarioID: number;
}

class BasicAnalysis extends Component<IBasicAnalysisProps> {

  componentWillReceiveProps(nextProps: any) {
    if (nextProps.scenarioID !== this.props.scenarioID) {
      const { dispatch, dealID, scenarioID } = nextProps;
      dispatch({ type: 'product/getPaymentChart', dealInfo: { deal_id: dealID } });
      dispatch({ type: 'product/getAssetPoolPaymentTrendChartData', payload: { deal_id: dealID, scenario_id: scenarioID } });
      dispatch({ type: 'product/getICOCTrendChartData', payload: { deal_id: dealID, scenario_id: scenarioID } });
      dispatch({ type: 'product/getAssetPoolInterestRateTrendChartData', payload: { deal_id: dealID, scenario_id: scenarioID } });
      dispatch({ type: 'product/getPaymentList', payload: { scenario_id: scenarioID } });
      // dispatch({ type: 'product/getComputationResultList', payload: { deal_id: dealID, scenario_id: scenarioID } });
    }
  }
  render() {
    return (
      <ABSContainer className="basic-analysis">
        <ABSMinContainer>
          <ComputationResult />
          <DrawResult />
          <RelationAndResult />
        </ABSMinContainer>
      </ABSContainer>
    );
  }
}

const mapStateToProps = ({ product, global }) => {
  const { scenarioID } = product;
  return {
    scenarioID,
    dealID: global.params.dealID,
  };
};

export default connect(mapStateToProps)(BasicAnalysis);