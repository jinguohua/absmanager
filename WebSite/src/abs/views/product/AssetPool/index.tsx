import React, { Component } from 'react';
import AssetPoolInfor from './assetPoolInfor';
import ABSContainer from '../../../../components/ABSContainer';
import AssetPoolChange from './assetPoolChange';
import AssetPoolPayment from './assetPoolPayment';
import RiskAnalysisChart from './riskAnalysisChart';
import DefaultSituation from './defaultSituation';
import DistributionModule from './distributionModule';
import DefaultAndPrepayment from './defaultAndPrepayment';
import BreachOfContract from './breachOfContract';
import { connect } from 'dva';
import ABSMinContainer from '../../../../components/ABSMinContainer';
import ABSNotSupport from '../../../../components/ABSNotSupport';
import './index.less';

interface IABSAssetPoolProps {
  isHasAssetPool?: boolean;
}

const mapStateToProps = ({ product }) => ({
  isHasAssetPool: product.isHasAssetPool,
});

@connect(mapStateToProps)
class AssetPool extends Component<IABSAssetPoolProps> {
  renderContent() {
    return (
      <>
        <AssetPoolInfor />
        <AssetPoolChange />
        <AssetPoolPayment />
        <RiskAnalysisChart />
        <DefaultAndPrepayment />
        <DefaultSituation />
        <BreachOfContract />
        <DistributionModule />
      </>
    );
  }

  renderNotSupport() {
    return <ABSNotSupport message="当前产品暂不支持资产池" />;
  }
  render() {
    const { isHasAssetPool } = this.props;
    return (
      <ABSContainer>
        <ABSMinContainer>
          {isHasAssetPool ? this.renderContent() : this.renderNotSupport()}
        </ABSMinContainer>
      </ABSContainer>
    );
  }
}

export default AssetPool;