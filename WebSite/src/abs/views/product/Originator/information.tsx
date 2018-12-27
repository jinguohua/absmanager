import React, { Component } from 'react';
import { connect } from 'dva';
import { Row, Col } from 'antd';
import ABSPanel from '../../../../components/ABSPanel';
import ABSLabelValueList from '../../../../components/ABSLabelValueList';
import commonUtils from '../../../../utils/commonUtils';
import './index.less';

export interface IInformationProps {
  dispatch: ({ }: any) => void;
  dealID: number;
  organizationID: number;
  originatorDetail: any;
}

class Information extends Component<IInformationProps, any> {
  componentDidMount() {
    const { dispatch, organizationID } = this.props;
    dispatch({
      type: 'product/getOriginatorDetail',
      payload: {
        organization_id: organizationID
      },
    });
  }
  renderInfo() {
    const { originatorDetail } = this.props;
    const newOriginatorDetail = originatorDetail ? originatorDetail : {};
    const list = [
      {
        title: '主体名称',
        content: commonUtils.formatContent(newOriginatorDetail.organization_full_name, false),
      },
      {
        title: '存贷款比率',
        content: commonUtils.formatContent(newOriginatorDetail.loan_deposit_ratio, false, true),
      },
      {
        title: '资产收益率',
        content: commonUtils.formatContent(newOriginatorDetail.total_return_on_assets, false, true),
      },
      {
        title: '拨备覆盖率',
        content: commonUtils.formatContent(newOriginatorDetail.provision_coverage_ratio, false, true),
      },
    ];
    const list1 = [
      {
        title: '主体评级',
        content: commonUtils.formatContent(newOriginatorDetail.rating, false),
      },
      {
        title: '总资产(亿)',
        content: commonUtils.formatContent(newOriginatorDetail.total_assets, false, false, true, 2, 8),
      },
      {
        title: '资本充足率',
        content: commonUtils.formatContent(newOriginatorDetail.capital_abundance_ratio, false, true),
      },
    ];
    const list2 = [
      {
        title: '截止日期',
        content: commonUtils.formatContent(newOriginatorDetail.as_of_date, false),
      },
      {
        title: '资产负债率',
        content: commonUtils.formatContent(newOriginatorDetail.originatorDetail, false, true),
      },
      {
        title: '不良贷款率',
        content: commonUtils.formatContent(newOriginatorDetail.npl_ratio, false, true),
      },
    ];
    return (
      <ABSPanel title="基本信息">
        <div className="deal-infor-basic">
          <Row>
            <Col span={8} className="basic-infor-first-label-list">
              <ABSLabelValueList list={list} />
            </Col>
            <Col span={8}>
              <ABSLabelValueList list={list1} />
            </Col>
            <Col span={8}>
              <ABSLabelValueList list={list2} />
            </Col>
          </Row>
        </div>
      </ABSPanel>
    );
  }
  render() {
    return (
      <div className="basic-infor">
        {this.renderInfo()}
      </div>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  const { originatorDetail } = product;
  return { originatorDetail, dealID: global.params.dealID, organizationID: global.params.organizationID };
}

export default connect(mapStateToProps)(Information); 