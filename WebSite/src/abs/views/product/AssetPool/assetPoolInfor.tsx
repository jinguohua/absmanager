import React, { Component } from 'react';
import { connect } from 'dva';
import { Row, Col } from 'antd';
import ABSPanel from '../../../../components/ABSPanel';
import ABSLabelValueList from '../../../../components/ABSLabelValueList';
import './assetPoolInfor.less';

class AssetPoolInfor extends Component<any, any> {
  componentDidMount() {
    const { dispatch, dealID } = this.props;
    dispatch({
      type: 'product/getAssetDetail',
      payload: {
        deal_id: dealID
      },
    });
  }
  labelValueList = () => {
    const { assetDetail } = this.props;
    const newAssetDetail = (assetDetail && assetDetail.asset_pool_detail_infos) ? assetDetail.asset_pool_detail_infos : [];
    newAssetDetail.map((item) => {
      item.key_values.map((childrenItem) => {
        childrenItem.title = childrenItem.label;
        childrenItem.content = childrenItem.value ? childrenItem.value : '--';
      });
    });
    return newAssetDetail;
  }
  renderInfo() {
    const newAssetDetail = this.labelValueList();
    return (
      newAssetDetail.map((item, index) => {
        return (
          <Col span={8} key={index}>
            <ABSPanel title={item.title}>
              <div className="asset-pool-infor">
                <ABSLabelValueList list={item.key_values} />
              </div>
            </ABSPanel>
          </Col>
        );
      })
    );
  }
  render() {
    return (
      <div className="asset-pool-infor">
        <div>
          <Row gutter={6}>
            {this.renderInfo()}
          </Row>
        </div>
      </div>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  return { assetDetail: product.assetDetail, dealID: global.params.dealID, };
}

export default connect(mapStateToProps)(AssetPoolInfor); 