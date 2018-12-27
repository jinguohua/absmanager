import React, { Component } from 'react';
import { connect } from 'dva';
import { Row, Col } from 'antd';
import ABSChart from '../../../../../components/ABSChart';
import ABSPanel from '../../../../../components/ABSPanel';
import { ABSStructure, INoteData } from '../../../../../components/ABSStructure';
import './index.less';

export interface IDealInformationProps {
  dispatch: ({ }: any) => void;
  detail: any;
  structureChart: any;
  paymentChart: any;
  dealID: number;
  securityID: number;
  paymentChartLoading: boolean;
}

export interface IDealInformationState {
}

class DealImages extends Component<IDealInformationProps, IDealInformationState> {

  componentDidMount() {
    const { dealID, securityID } = this.props;
    this.props.dispatch({ type: 'product/getPaymentChart', dealInfo: { deal_id: dealID } });
    this.props.dispatch({ type: 'product/getStructureChart', dealInfo: { deal_id: dealID, security_id: securityID } });
  }

  render() {
    const { structureChart, paymentChart, paymentChartLoading } = this.props;
    const { notes } = structureChart;
    const data: INoteData[] = notes ? notes.map((item) => {
      let temp: INoteData;
      temp = item;
      temp.noteId = item.note_id;
      temp.isEquity = item.is_equity;
      temp.rating = item.rating;
      return temp;
    }) : null;
    return (
      <ABSPanel title="要素图" removePadding={true}>
        <Row className="deal-info-image">
          <Col className="deal-info-image-left" span={8}>
            <ABSStructure data={data} />
          </Col>

          <Col className="deal-info-image-left draw-result-chart" span={16}>
            <ABSChart config={paymentChart} loading={paymentChartLoading} />
          </Col>
        </Row>
      </ABSPanel>
    );
  }
}

const mapStateToProps = ({ global, product }) => {
  const { structureChart, paymentChart, paymentChartLoading } = product;
  return {
    dealID: global.params.dealID,
    securityID: global.params.securityID,
    structureChart,
    paymentChart,
    paymentChartLoading
  };
};

export default connect(mapStateToProps)(DealImages);