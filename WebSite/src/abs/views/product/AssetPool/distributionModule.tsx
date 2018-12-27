import React from 'react';
import { Row, Col } from 'antd';
import { connect } from 'dva';
import ABSPanel from '../../../../components/ABSPanel';
import ABSListView from '../../../../components/ABSList';
import ABSChart from '../../../../components/ABSChart';
import './distributionModule.less';
import { getColumnChartConfig, getlineColumnChartConfig } from './utils';
import CommonUtils from '../../../../utils/commonUtils';
import PerfectScrollbar from 'react-perfect-scrollbar';

export interface IDistributionModuleProps {
  type: 'borrower-area' | 'borrower-age' | 'loan-contract-term' | 'asset-pool';
  // loading?: boolean;
}

class DistributionModule extends React.Component<any, any> {

  componentDidMount() {
    const { dealID } = this.props;
    this.props.dispatch({
      type: 'product/getDistributeList',
      payload: {
        deal_id: dealID
      },
    });
  }

  restructureDatas = () => {
    const { distributeList } = this.props;
    const distributeDatas = distributeList ? distributeList : [];
    distributeDatas.map((dom) => {
      dom.chart = getColumnChartConfig(dom.chart); // 重构chart(饼图)数据
      if (dom.group_type === 'concentration_ratio') {
        dom.concentration_chart = getlineColumnChartConfig(dom.concentration_chart); // 重构chart(柱状图)数据
      }
      dom.table = this.restructureTableDatas(dom.table);
    });
    return distributeDatas;
  }

  // 重构table数据
  restructureTableDatas = (data) => {
    data.labels.map((item) => {
      item.dataIndex = item.name;
      // item.key = item.name;
      if (item.data_type !== 'string') {
        item.className = 'abs-right';
        if (item.dataIndex === 'balance') {
          item.render = text => CommonUtils.formatContent(text, true, null, null, null, 4);
        } else if (item.dataIndex === 'asset_count') {
          item.render = text => CommonUtils.formatContent(text, true, null, null, 0);
        }
      }
    });
    return data;
  }

  render() {
    // const { loading } = this.props;
    const distributeDatas = this.restructureDatas();

    return (
      distributeDatas.map((item, index) => {
        if (item.group_type === 'concentration_ratio') {
          return (
            <div className="distribution-module-third" key={index}>
              <ABSPanel title={item.distribute_title} removePadding={true}>
                <PerfectScrollbar>
                  <Row>
                    <Col span={8}>
                      <ABSChart config={item.chart} />
                    </Col>
                    <Col span={8}>
                      <PerfectScrollbar>
                        <ABSListView
                          type="default"
                          tableWidth="auto"
                          bordered={false}
                          columnsData={item.table.labels}
                          contentData={item.table.data}
                          paginationData={false}
                          loading={false}
                        />
                      </PerfectScrollbar>
                    </Col>
                    <Col span={8}>
                      <div className="abs-chart-top">
                        <span>集中度</span>
                        <span>{CommonUtils.formatContent(item.concentration_ratio, null, null, null, 0)}</span>
                        <span className="abs-chart-top-text">平均值</span>
                        <span>{CommonUtils.formatContent(item.avg_ratio, null, null, null, 0)}</span>
                      </div>
                      <ABSChart config={item.concentration_chart} />
                    </Col>
                  </Row>
                </PerfectScrollbar>
              </ABSPanel>
            </div>
          );
        }
        return (
          <div className="distribution-module-two" key={index}>
            <ABSPanel title={item.distribute_title} removePadding={true}>
              <Row>
                <Col span={8}>
                  <ABSChart config={item.chart} />
                </Col>
                <Col span={16}>
                  <ABSListView
                    type="default"
                    tableWidth="auto"
                    bordered={false}
                    columnsData={item.table.labels}
                    contentData={item.table.data}
                    paginationData={false}
                    loading={false}
                  />
                </Col>
              </Row>
            </ABSPanel>
          </div>
        );
      })
    );
  }
}

function mapStateToProps({ product, loading, global }: any) {
  return { distributeList: product.distributeList, dealID: global.params.dealID, }; // , loading: loading.models.product
}

export default connect(mapStateToProps)(DistributionModule);