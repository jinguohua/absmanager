import React, { Component } from 'react';
import { connect } from 'dva';
import { Row, Col } from 'antd';
import ABSPanel from '../../../../components/ABSPanel';
import ABSListView from '../../../../components/ABSList';
import PerfectScrollbar from 'react-perfect-scrollbar';
import commonUtils from '../../../../utils/commonUtils';
import './breachOfContract.less';

let columnsDatas: any = [];

class DefaultSituation extends Component<any, any> {
  
  componentDidMount() {
    const { dispatch, dealID } = this.props;
    dispatch({ type: 'product/getAssetRiskList', payload: { deal_id: dealID } });
  }

  defaultSituationDatas = (riskListDatas) => {
    const defaultData = riskListDatas.default_data ? riskListDatas.default_data : [];
    const dataLables = defaultData.lables ? defaultData.lables : [];
    columnsDatas = [];
    dataLables.forEach(item => {
      const columns = {
        title: item.title,
        key: item.name,
        dataIndex: item.name,
        className: 'abs-right',
        render: (text) => commonUtils.formatContent(text, true, true, false, 2, 0)
      };
      if (item.name === 'date') {
        columns.dataIndex = 'date';
        columns.render = commonUtils.formatContent(columns.dataIndex, true, false, false, 0, 0);
      }
      columnsDatas.push(columns);
    });
    return columnsDatas;
  }

  headerContentLineChange(firstText: string, secondText: string) {
    return (
      <div className="abs-right">{firstText}<br/>{secondText}</div>
    );
  }
  renderProject() {
    const { riskList } = this.props;
    const riskListDatas = riskList ? riskList : {};
    const defaultData = riskListDatas.default_data ? riskListDatas.default_data : [];
    const contentDatas = defaultData.data ? defaultData.data : [];
    if (defaultData === null) {
      return;
    }
    return (
      <ABSPanel title="违约处置情况" removePadding={true}>
        <div className="risk-analysis">
          <Row>
            <Col span={24}>
            <PerfectScrollbar>
              <ABSListView
                type="default"
                tableWidth="100%"
                bordered={false}
                columnsData={this.defaultSituationDatas(riskListDatas)}
                contentData={contentDatas}
                loading={false}
              />
            </PerfectScrollbar>
            </Col>
          </Row>
        </div>
      </ABSPanel>
    );
  }
  render() {
    return (
      <div className="asset-pool-risk-analysis">
        {this.renderProject()}
      </div>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  return { riskList: product.riskList, dealID: global.params.dealID, };
}

export default connect(mapStateToProps)(DefaultSituation); 