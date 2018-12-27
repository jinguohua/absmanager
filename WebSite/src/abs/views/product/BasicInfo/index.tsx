import React, { Component } from 'react';
import DealInformation from './DealInformation';
import Tab from './Tab';
import { connect } from 'dva';
import ABSContainer from '../../../../components/ABSContainer';
import './index.less';

export interface IBasicInfoProps {
  dealID?: number;
  dispatch: ({ }: any) => void;
}

export interface IBasicInfoState {
}
class BasicInfo extends Component<IBasicInfoProps, IBasicInfoState> {
  componentDidMount() {
    const { dealID } = this.props;
    this.props.dispatch({ type: 'product/getDetail', dealInfo: { deal_id: dealID } });
  }

  render() {
    return (
      <div className="product-basic-info">
        <ABSContainer className="product-basic-info-container">
          <DealInformation />
        </ABSContainer>
        <Tab />
      </div>
    );
  }
}

const mapStateToProps = ({ global }) => {
  return { dealID: global.params.dealID };
};

export default connect(mapStateToProps)(BasicInfo);