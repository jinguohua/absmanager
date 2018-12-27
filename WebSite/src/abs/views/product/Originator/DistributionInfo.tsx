import React, { Component } from 'react';
import './DistributionInfo.less';
import { connect } from 'dva';
import ABSPanel from '../../../../components/ABSPanel';
import ABSChart from '../../../../components/ABSChart';
import { Col, Row } from 'antd';

export interface IDistributionInfoProps {
  dispatch: ({ }: any) => void;
  toalAsset: any;
  toalAssetLoading: boolean;
  liabilityAssetRatio: any;
  liabilityAssetRatioLoading: boolean;
  assetYield: any;
  assetYieldLoading: boolean;
  capitalAbundanceRatio: any;
  capitalAbundanceRatioLoading: boolean;
  coreCapitalAbundanceRatio: any;
  coreCapitalAbundanceRatioLoading: boolean;
  nplRatio: any;
  nplRatioLoading: boolean;
  dealID: number;
}

export interface IDistributionInfoState {
}

class DistributionInfo extends Component<IDistributionInfoProps, IDistributionInfoState> {

  componentDidMount() {
    const { dealID } = this.props;
    this.props.dispatch({ type: 'product/getToalAsset', chartInfo: { deal_id: dealID } });
    this.props.dispatch({ type: 'product/getLiabilityAssetRatio', chartInfo: { deal_id: dealID } });
    this.props.dispatch({ type: 'product/getAssetYield', chartInfo: { deal_id: dealID } });
    this.props.dispatch({ type: 'product/getCapitalAbundanceRatio', chartInfo: { deal_id: dealID } });
    this.props.dispatch({ type: 'product/getCoreCapitalAbundanceRatio', chartInfo: { deal_id: dealID } });
    this.props.dispatch({ type: 'product/getPlRatio', chartInfo: { deal_id: dealID } });
  }

  renderImage() {
    const {
      toalAsset, nplRatio, assetYield,
      liabilityAssetRatio,
      capitalAbundanceRatio,
      coreCapitalAbundanceRatio,
      toalAssetLoading,
      liabilityAssetRatioLoading,
      assetYieldLoading,
      capitalAbundanceRatioLoading,
      coreCapitalAbundanceRatioLoading,
      nplRatioLoading,
    } = this.props;   
    return (
      <ABSPanel title="分布图" removePadding={true}>
        <div className="distribution-infomation">
          <Row className="distribution-info-image-top">
            <Col span={12} className="draw-result-chart" style={{ paddingRight: '3px' }}>
              <ABSChart config={toalAsset} loading={toalAssetLoading} />
            </Col>
            <Col span={12} className="draw-result-chart" style={{ paddingLeft: '3px' }}>
              <ABSChart config={liabilityAssetRatio} loading={liabilityAssetRatioLoading} />
            </Col>
          </Row>

          <Row className="distribution-info-image">
            <Col span={12} className="draw-result-chart" style={{ paddingRight: '3px' }}>
              <ABSChart config={assetYield} loading={assetYieldLoading} />
            </Col>

            <Col span={12} className="draw-result-chart" style={{ paddingLeft: '3px' }}>
              <ABSChart config={capitalAbundanceRatio} loading={capitalAbundanceRatioLoading} />
            </Col>
          </Row>

          <Row className="distribution-info-image">
            <Col span={12} className="draw-result-chart" style={{ paddingRight: '3px' }}>
              <ABSChart config={coreCapitalAbundanceRatio} loading={coreCapitalAbundanceRatioLoading} />
            </Col>
            <Col span={12} className="draw-result-chart" style={{ paddingLeft: '3px' }}>
              <ABSChart config={nplRatio} loading={nplRatioLoading} />
            </Col>
          </Row>
        </div >
      </ABSPanel >
    );
  }

  render() {
    return (
      <div className="distribution-info">
        {this.renderImage()}
      </div>
    );
  }
}

const mapStateToProps = ({ global, product }) => {
  const {
    toalAsset, nplRatio, assetYield,
    liabilityAssetRatio,
    coreCapitalAbundanceRatio,
    capitalAbundanceRatio,
    toalAssetLoading,
    liabilityAssetRatioLoading,
    assetYieldLoading,
    capitalAbundanceRatioLoading,
    coreCapitalAbundanceRatioLoading,
    nplRatioLoading,
  } = product;
  return {
    dealID: global.params.dealID,
    toalAsset,
    liabilityAssetRatio,
    assetYield,
    capitalAbundanceRatio,
    coreCapitalAbundanceRatio,
    nplRatio,
    toalAssetLoading,
    liabilityAssetRatioLoading,
    assetYieldLoading,
    capitalAbundanceRatioLoading,
    coreCapitalAbundanceRatioLoading,
    nplRatioLoading,
  };
};

export default connect(mapStateToProps)(DistributionInfo);