import React, { Component } from 'react';
import { connect } from 'dva';
import ABSPanel from '../../../../../components/ABSPanel';
import ABSEmphasizeText from '../../../../../components/ABSEmphasizeText';
import ABSLabelValueList from '../../../../../components/ABSLabelValueList';
import { getBasicData } from '../util';
import './index.less';
import ABSLink from '../../../../../components/ABSLink';
import RouteConfig from '../../../../config/routeConfig';

export interface IDealInformationProps {
  detail: any;
  loading?: boolean;
}

export interface IDealInformationState {
}

class DealInfo extends Component<IDealInformationProps, IDealInformationState> {

  render() {
    const { loading } = this.props;
    const { basic_info } = this.props.detail ? this.props.detail : '';
    const { list, list1, list2 } = getBasicData(basic_info);
    const { deal_type_id } = basic_info ? basic_info : '';
    list1[1].content = <ABSLink to={`${RouteConfig.productList}?filter_query_list[0][key]=5&filter_query_list[0][value][0]=${deal_type_id}`}>{list1[1].content}</ABSLink>;
    list2[0].content = <ABSEmphasizeText title={list2[0].content} style={{ color: 'red' }} />;
    return (
      <ABSPanel title="基本要素" loading={loading}>
        <div className="deal-info-basic">
          <ABSLabelValueList list={list} />
          <ABSLabelValueList list={list1} />
          <ABSLabelValueList list={list2} />
        </div>
      </ABSPanel>
    );
  }
}

const mapStateToProps = ({ product, loading }) => {
  return { detail: product.detail, loading: loading.models.product };

};

export default connect(mapStateToProps)(DealInfo);