import React from 'react';
// import ABSList from '../../../../components/ABSList';
// import ABSPagination from '../../../../components/ABSPagination';
import commonUtils from '../../../../utils/commonUtils';
import './index.less';
import ABSListContainer from '../../../../components/ABSListContainer';

export interface IABSTableProps {
  
}
 
export interface IABSTableState {
  
}
 
const columnsData = [
  {
    title: '证券简称',
    dataIndex: 'short_name',
    key: 'short_name',
    render: (text, record) => text
  }, {
    title: '证券代码',
    dataIndex: 'code',
    key: 'code',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '证券类型',
    dataIndex: 'type',
    key: 'type',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '还本方式',
    key: 'principal_repayment',
    dataIndex: 'principal_repayment',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    className: 'abs-right',
    title: '发行量(亿)',
    dataIndex: 'notional',
    key: 'notional',
    render: (text: string) => commonUtils.formatContent(text, true, false, true, 2, 8)
  }, {
    className: 'abs-right',
    title: '剩余量(亿)',
    dataIndex: 'principal',
    key: 'principal',
    render: (text: string) => commonUtils.formatContent(text, true, false, true, 2, 8)
  }, {
    className: 'abs-right',
    title: '定价',
    dataIndex: 'npv',
    key: 'npv',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    className: 'abs-right',
    title: 'WAL(年)',
    dataIndex: 'expected_life',
    key: 'expected_life',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '起息日',
    dataIndex: 'closing_date',
    key: 'closing_date',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '预计到期日',
    dataIndex: 'expected_maturity_date',
    key: 'expected_maturity_date',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    className: 'abs-right',
    title: '发行利率(%)',
    dataIndex: 'coupon',
    key: 'coupon',
    render: (text: string) => commonUtils.formatContent(text, true, true)
  }, {
    title: '付息频率',
    dataIndex: 'frequency',
    key: 'frequency',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '交易场所',
    dataIndex: 'exchange',
    key: 'exchange',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '市场分类',
    dataIndex: 'market_type',
    key: 'market_type',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '产品分类',
    dataIndex: 'deal_type',
    key: 'deal_type',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '产品细分',
    dataIndex: 'asset_sub_category',
    key: 'asset_sub_category',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '最新评级1',
    dataIndex: 'latest_rating1',
    key: 'latest_rating1',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '最新评级2',
    dataIndex: 'latest_rating2',
    key: 'latest_rating2',
    render: (text: string) => commonUtils.formatContent(text, true)
  }
];

class ABSUIDealList extends React.Component<IABSTableProps, IABSTableState> {
  render() { 
    return (
      <div className="absui-table">
        <ABSListContainer
          title="证券列表" 
          columnsData={columnsData} 
          actionType="product/getDealProductList" 
          payload={{ filter_type_id: 1, page_size: 20 }}
          model="product.dealProductList"
        />
      </div>
    );
  }
}
 
export default ABSUIDealList;