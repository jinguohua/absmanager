import React, { Component } from 'react';
import { connect } from 'dva';
import ABSPanel from '../../../../components/ABSPanel';
import ABSChart from '../../../../components/ABSChart';
import { getLineChartConfig } from './utilChart';
import './assetPoolPayment.less';

export interface IAssetPoolPaymentProps {
  dispatch: ({ }: any) => void;
  paymentChartList: any;
  paymentChartLoading: boolean;
  dealID: any;
  scenarioID: string | null;
  isHasAssetChart?: boolean;
}
class AssetPoolPayment extends Component<IAssetPoolPaymentProps, any> {
  componentDidMount() {
    const { dispatch, dealID, scenarioID } = this.props;
    dispatch({
      type: 'product/getAssetPaymentChart',
      payload: {
        deal_id: dealID,
        scenario_id: scenarioID,
      },
    });
  }

  renderContent() {
    const { paymentChartList, paymentChartLoading } = this.props;

    return (
      <div className="asset-pool-payment">
        <ABSPanel title="资产池偿付" removePadding={true}>
          <ABSChart config={getLineChartConfig(paymentChartList)} loading={paymentChartLoading} />
        </ABSPanel>
      </div>
    );
  }

  render() {
    const { isHasAssetChart } = this.props;
    if (isHasAssetChart) {
      return this.renderContent();
    }

    return null;
  }
}

const mapStateToProps = ({ product, global }) => {
  const { paymentChartList, paymentChartLoading, scenarioID, isHasAssetChart } = product;
  return {
    paymentChartList,
    paymentChartLoading,
    dealID: global.params.dealID,
    isHasAssetChart: isHasAssetChart,
    scenarioID,
  };
};
export default connect(mapStateToProps)(AssetPoolPayment);