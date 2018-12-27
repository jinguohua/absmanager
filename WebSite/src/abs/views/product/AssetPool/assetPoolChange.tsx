import React, { Component } from 'react';
import { connect } from 'dva';
import { Row, Col } from 'antd';
import ABSPanel from '../../../../components/ABSPanel';
import ABSList from '../../../../components/ABSList';
import commonUtils from '../../../../utils/commonUtils';

class AssetPoolChange extends Component<any, any> {
  renderProject() {
    const { changeList, dealID } = this.props;
    const payload = { deal_id: dealID };
    const columnsData = [
      {
        title: '归集日期',
        dataIndex: 'date',
        key: 'date',
        render: (text, record, index) => {
          if (index === (changeList.length - 1)) {
            text = '初始资产池';
          }
          return text;
        }
      }, {
        className: 'abs-right',
        title: '余额(万)',
        dataIndex: 'balance',
        key: 'balance',
        render: (text) => commonUtils.formatContent(text, true, null, null, null, 4)
      }, {
        className: 'abs-right',
        title: '资产个数',
        dataIndex: 'asset_count',
        key: 'asset_count',
        render: text => commonUtils.formatContent(text, true, false, true, 0, 0)
      }, {
        className: 'abs-right',
        title: '平均利率',
        key: 'wac',
        dataIndex: 'wac',
      }, {
        className: 'abs-right',
        title: '平均期限(年)',
        dataIndex: 'wal',
        key: 'wal',
        render: text => commonUtils.formatContent(text, true, false, true)
      }, {
        className: 'abs-right',
        title: '利息收入(万)',
        dataIndex: 'interest_collection',
        key: 'interest_collection',
        render: (text) => commonUtils.formatContent(text, true, false, true, 2, 4)
      }, {
        className: 'abs-right',
        title: '本金收入(万)',
        dataIndex: 'principal_collection',
        key: 'principal_collection',
        render: (text) => commonUtils.formatContent(text, true, false, true, 2, 4)
      },
    ];
    return (
      <ABSPanel title="资产池变化" removePadding={true}>
        <div className="asset-pool-change-list">
          <Row>
            <Col span={24}>
              <ABSList
                actionType="product/getAssetChangeList"
                payload={payload}
                columnsData={columnsData}
                model="product.changeList"
              />
            </Col>
          </Row>
        </div>
      </ABSPanel>
    );
  }
  render() {
    return (
      <div className="asset-pool-change">
        {this.renderProject()}
      </div>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  return {
    changeList: product.changeList,
    dealID: global.params.dealID,
  };
}

export default connect(mapStateToProps)(AssetPoolChange);