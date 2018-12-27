import React, { Component } from 'react';
import { connect } from 'dva';
import { Row, Col } from 'antd';
import ABSPanel from '../../../../components/ABSPanel';
import ABSChart from '../../../../components/ABSChart';
import { getPrepaymentChart } from './utilChart';

class RiskAnalysisChart extends Component<any, any> {
  componentDidMount() {
    const { dispatch, dealID } = this.props;
    dispatch({
      type: 'product/getAssetPrepaymentChart',
      payload: {
        deal_id: dealID
      },
    });
    dispatch({
      type: 'product/getAssetDefaultChart',
      payload: {
        deal_id: dealID
      },
    });
  }

  renderProject() {
    const { assetPrepaymentChart,
      assetDefaultChart,
      assetPrepaymentChartLoading,
      assetDefaultChartLoading } = this.props;
    if (!assetPrepaymentChart && !assetDefaultChart) {
      return '';
    } else {
      // const assetPrepaymentCharts = assetPrepaymentChart ? assetPrepaymentChart : [];
      // const assetDefaultCharts = assetDefaultChart ? assetDefaultChart : [];
      return (
        <div className="asset-pool-payment">
          <ABSPanel title="资产池风险分析图" removePadding={true}>
            <Row gutter={6}>
              <Col span={12}>
                <ABSChart config={getPrepaymentChart(assetPrepaymentChart)} loading={assetPrepaymentChartLoading} />
              </Col>
              <Col span={12}>
                <ABSChart config={getPrepaymentChart(assetDefaultChart)} loading={assetDefaultChartLoading} />
              </Col>
            </Row>
          </ABSPanel>
        </div>
      );
    }
  }
  render() {
    return (
      <div className="asset-pool-risk-analysis-chart">
        {this.renderProject()}
      </div>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  return {
    assetPrepaymentChart: product.assetPrepaymentChart,
    assetDefaultChart: product.assetDefaultChart,
    dealID: global.params.dealID,
  };
}

export default connect(mapStateToProps)(RiskAnalysisChart); 