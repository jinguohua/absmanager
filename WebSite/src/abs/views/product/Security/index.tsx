import React from 'react';
import { connect } from 'dva';
import ABSList from '../../../../components/ABSList';
import ABSPanel from '../../../../components/ABSPanel';
import ABSContainer from '../../../../components/ABSContainer';
import ABSMinContainer from '../../../../components/ABSMinContainer';
import commonUtils from '../../../../utils/commonUtils';
import ABSLink from '../../../../components/ABSLink';
import RouteConfig from '../../../config/routeConfig';
import './index.less';

class Security extends React.Component<any, any> {
  handleRowDelete = () => {
    const { dispatch, dealID } = this.props;
    dispatch({
      type: 'product/getSecurityList',
      payload: { deal_id: dealID },
    });
  }

  render() {
    const { dealID, securityList } = this.props;
    const columnsData = [
      {
        title: '证券简称',
        dataIndex: 'description',
        key: 'description',
        fixed: 'left',
        render: (text, record) => (
          <ABSLink to={RouteConfig.investmentSecurityInfo + record.note_id} >{text}</ABSLink>
        ),
      }, {
        className: 'abs-right',
        title: '证券代码',
        dataIndex: 'security_code',
        key: 'security_code',
        fixed: 'left',
        render: (text) => commonUtils.formatContent(text, true)
      }, {
        className: 'abs-right',
        title: '发行量(万)',
        dataIndex: 'notional',
        key: 'notional',
        render: (text) => commonUtils.formatContent(text, true, false, true, 2, 4)
      }, {
        className: 'abs-right',
        title: '存量(万)',
        key: 'principal',
        dataIndex: 'principal',
        render: (text) => commonUtils.formatContent(text, true, false, true, 2, 4)
      }, {
        title: '利率类型',
        key: 'coupon_type',
        dataIndex: 'coupon_type',
        render: (text) => commonUtils.formatContent(text, true)
      }, {
        className: 'abs-right',
        title: '发行利率',
        key: 'coupon',
        dataIndex: 'coupon',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: '当前利率',
        key: 'current_coupon',
        dataIndex: 'current_coupon',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: '初始基准',
        key: 'base_coupon',
        dataIndex: 'base_coupon',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: '初始利差',
        key: 'base_interest',
        dataIndex: 'base_interest',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        title: '还本方式',
        key: 'repayment_of_principal',
        dataIndex: 'repayment_of_principal',
        render: (text) => commonUtils.formatContent(text, true)
      }, {
        className: 'abs-right',
        title: '定价',
        key: 'clean_price',
        dataIndex: 'clean_price',
        render: (text) => commonUtils.formatContent(text, true, false, true, 4)
      }, {
        className: 'abs-right',
        title: '收益率',
        key: 'expected_rate_securities',
        dataIndex: 'expected_rate_securities',
        render: (text) => commonUtils.formatContent(text, true, true, false, 4)
      }, {
        className: 'abs-right',
        title: '剩余面额(元)',
        key: 'remaining_principal_amount',
        dataIndex: 'remaining_principal_amount',
        render: (text) => commonUtils.formatContent(text, true, false, true, 4)
      }, {
        title: '预计到期日',
        key: 'expected_maturity_date',
        dataIndex: 'expected_maturity_date',
        render: (text) => commonUtils.formatContent(text, true)
      }, {
        className: 'abs-right',
        title: '加权年限',
        key: 'current_wal',
        dataIndex: 'current_wal',
        render: (text) => commonUtils.formatContent(text, true)
      }, {
        title: `${securityList.rating_column_name}(原始)`,
        key: 'rating_pause',
        dataIndex: 'rating_pause',
        render: (text) => commonUtils.formatContent(text, true)
      }, {
        title: `${securityList.rating_column_name}(当前)`,
        key: 'current_rating_pause',
        dataIndex: 'current_rating_pause',
        render: (text) => commonUtils.formatContent(text, true)
      }, {
        className: 'abs-right',
        title: '初始比例',
        key: 'notional_pct',
        dataIndex: 'notional_pct',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: '当前比例',
        key: 'principal_pct',
        dataIndex: 'principal_pct',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: '参考利率',
        key: 'current_refer_coupon',
        dataIndex: 'current_refer_coupon',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: '参考利差',
        key: 'current_refer_interest',
        dataIndex: 'current_refer_interest',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: 'CDR临界值',
        key: 'cdr',
        dataIndex: 'cdr',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: '本金覆盖率',
        key: 'principal_coverage',
        dataIndex: 'principal_coverage',
        render: (text) => commonUtils.formatContent(text, true, true)
      }, {
        className: 'abs-right',
        title: '利息覆盖率',
        key: 'interest_coverage',
        dataIndex: 'interest_coverage',
        render: (text) => commonUtils.formatContent(text, true, true)
      }
    ];

    return (
      <ABSContainer>
        <ABSMinContainer>
          <ABSPanel title="产品证券" removePadding={true}>
            <ABSList
              columnsData={columnsData}
              actionType="product/getSecurityList"
              payload={{ deal_id: dealID }}
              model="product.securityList.note_list"
              scroll={{ x: 2000 }}
            />
          </ABSPanel>
        </ABSMinContainer>
      </ABSContainer>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  return { dealID: global.params.dealID, securityList: product.securityList };
}

export default connect(mapStateToProps)(Security); 
