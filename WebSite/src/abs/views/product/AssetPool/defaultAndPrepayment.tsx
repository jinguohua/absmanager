import React, { Component } from 'react';
import { connect } from 'dva';
import { Row, Col } from 'antd';
import ABSPanel from '../../../../components/ABSPanel';
import ABSList from '../../../../components/ABSList';
import commonUtils from '../../../../utils/commonUtils';

class DefaultAndPrepayment extends Component<any, any> {
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
        title: '12月违约率',
        dataIndex: 'recent_year_default_rate',
        key: 'recent_year_default_rate',
        render: (text) => commonUtils.formatContent(text, true, true, false, 2, 0)
      }, {
        className: 'abs-right',
        title: '12月提前偿还率',
        dataIndex: 'recent_year_prepayment_rate',
        key: 'recent_year_prepayment_rate',
        render: (text) => commonUtils.formatContent(text, true, true, false, 2, 0)
      }, {
        className: 'abs-right',
        title: '年化违约率',
        dataIndex: 'annualized_default_rate',
        key: 'annualized_default_rate',
        render: (text) => commonUtils.formatContent(text, true, true, false, 2, 0)
      }, {
        className: 'abs-right',
        title: '年化提前偿还率',
        dataIndex: 'annualized_prepayment_rate',
        key: 'annualized_prepayment_rate',
        render: (text) => commonUtils.formatContent(text, true, true, false, 2, 0)
      }, {
        className: 'abs-right',
        title: '累计违约率',
        dataIndex: 'cum_default_rate',
        key: 'cum_default_rate',
        render: (text) => commonUtils.formatContent(text, true, true, false, 2, 0)
      }, {
        className: 'abs-right',
        title: '累计提前偿还率',
        dataIndex: 'cum_prepayment_rate',
        key: 'cum_prepayment_rate',
        render: (text) => commonUtils.formatContent(text, true, true, false, 2, 0)
      },
    ];
    return (
      <ABSPanel title="违约与早偿率" removePadding={true}>
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

export default connect(mapStateToProps)(DefaultAndPrepayment);