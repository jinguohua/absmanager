import React from 'react';
import ABSPanel from '../../../../components/ABSPanel';
import ABSChart from '../../../../components/ABSChart';
import './drawResult.less';
import { connect } from 'dva';
import { Row, Col } from 'antd';

export interface IDrawResultProps {
  paymentChart: any;
  paymentChartLoading: boolean;
  assetPoolPaymentTrendChartData: any;
  assetPoolPaymentTrendChartDataLoading: boolean;
  ICOCTrendChartData: any;
  ICOCTrendChartDataLoading: boolean;
  assetPoolInterestRateTrendChartData: any;
  assetPoolInterestRateTrendChartDataLoading: boolean;
  dispatch: any;
  dealID: number;
  scenarioID: number;
  showExtraChart: boolean;
}

export interface IDrawResultState {
}

class DrawResult extends React.PureComponent<IDrawResultProps, IDrawResultState> {
  componentDidMount() {
    const { dispatch, dealID, scenarioID, showExtraChart } = this.props;
    dispatch({ type: 'product/getPaymentChart', dealInfo: { deal_id: dealID } });
    dispatch({ type: 'product/getAssetPoolPaymentTrendChartData', payload: { deal_id: dealID, scenario_id: scenarioID } });
    if (showExtraChart) {
      dispatch({ type: 'product/getICOCTrendChartData', payload: { deal_id: dealID, scenario_id: scenarioID } });
      dispatch({ type: 'product/getAssetPoolInterestRateTrendChartData', payload: { deal_id: dealID, scenario_id: scenarioID } });
    }
  }

  render() {
    const {
      paymentChart,
      paymentChartLoading,
      assetPoolPaymentTrendChartData,
      assetPoolPaymentTrendChartDataLoading,
      ICOCTrendChartData,
      ICOCTrendChartDataLoading,
      assetPoolInterestRateTrendChartData,
      assetPoolInterestRateTrendChartDataLoading,
      showExtraChart
    } = this.props;
    const paymentChartData = paymentChart;
    paymentChartData.chart = {};
    return (
      <ABSPanel title="绘图结果" removePadding={true}>
        <Row className="draw-result" style={{ marginBottom: '6px' }}>
          <Col className="draw-result-chart" span={12} style={{ paddingRight: '3px' }}>
            <ABSChart config={paymentChartData} loading={paymentChartLoading} />
          </Col>
          <Col className="draw-result-chart" span={12} style={{ paddingLeft: '3px' }}>
            <ABSChart config={assetPoolPaymentTrendChartData} loading={assetPoolPaymentTrendChartDataLoading} />
          </Col>
        </Row>
        {showExtraChart ?
          (<Row className="draw-result">
            <Col span={12} className="draw-result-chart" style={{ paddingRight: '3px' }}>
              <ABSChart config={ICOCTrendChartData} loading={ICOCTrendChartDataLoading} />
            </Col>
            <Col span={12} className="draw-result-chart" style={{ paddingLeft: '3px' }}>
              <ABSChart config={assetPoolInterestRateTrendChartData} loading={assetPoolInterestRateTrendChartDataLoading} />
            </Col>
          </Row>) : <div />}
      </ABSPanel>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  const {
    assetPoolPaymentTrendChartData,
    assetPoolPaymentTrendChartDataLoading,
    ICOCTrendChartData,
    ICOCTrendChartDataLoading,
    assetPoolInterestRateTrendChartData,
    assetPoolInterestRateTrendChartDataLoading,
    paymentChart,
    paymentChartLoading,
    scenarioID,
    showExtraChart
  } = product;
  return {
    assetPoolPaymentTrendChartData,
    assetPoolPaymentTrendChartDataLoading,
    ICOCTrendChartData,
    ICOCTrendChartDataLoading,
    assetPoolInterestRateTrendChartData,
    assetPoolInterestRateTrendChartDataLoading,
    paymentChart,
    paymentChartLoading,
    dealID: global.params.dealID,
    scenarioID,
    showExtraChart
  };
}

export default connect(mapStateToProps)(DrawResult);