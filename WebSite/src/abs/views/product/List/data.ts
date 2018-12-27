
import commonUtils from '../../../../utils/commonUtils';
import { addLink, addSecurityInfoLink } from './index';

export const securityColumnsData = [
  {
    title: '证券简称',
    dataIndex: 'short_name',
    key: 'short_name',
    render: (text, record) => addSecurityInfoLink(text, record)
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
    title: '交易场所',
    dataIndex: 'exchange',
    key: 'exchange',
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
    title: '存量(亿)',
    dataIndex: 'principal',
    key: 'principal',
    render: (text: string) => commonUtils.formatContent(text, true, false, true, 2, 8)
  }, {
    className: 'abs-right',
    title: '发行利率(%)',
    dataIndex: 'coupon',
    key: 'coupon',
    render: (text: string) => commonUtils.formatContent(text, true, true, false, 4)
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
    title: '付息频率',
    dataIndex: 'frequency',
    key: 'frequency',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    className: 'abs-right',
    title: '定价',
    dataIndex: 'npv',
    key: 'npv',
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
export const productColumnsData = [
  {
    title: '产品简称',
    dataIndex: 'short_name',
    key: 'short_name',
    render: (text, record) => addLink(text, record)
  }, {
    title: '产品名称',
    dataIndex: 'full_name',
    key: 'full_name',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '当前状态',
    dataIndex: 'current_status',
    key: 'current_status',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '交易场所',
    dataIndex: 'exchange',
    key: 'exchange',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    className: 'abs-right',
    title: '发行金额(亿)',
    dataIndex: 'total_offering',
    key: 'total_offering',
    render: (text: string) => commonUtils.formatContent(text, true, false, true, 2, 8)
  }, {
    title: '发起/原始权益人',
    dataIndex: 'originator',
    key: 'originator',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '循坏购买',
    dataIndex: 'is_cycle_purchase',
    key: 'is_cycle_purchase',
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
    title: '年份',
    dataIndex: 'year',
    key: 'year',
    render: (text: string) => commonUtils.formatContent(text, true, false, false, 0)
  }, {
    title: '监管机构',
    key: 'regulator',
    dataIndex: 'regulator',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '发行方式',
    dataIndex: 'issue_type',
    key: 'issue_type',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '起息日',
    dataIndex: 'closing_date',
    key: 'closing_date',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '法定到期日',
    dataIndex: 'legal_maturity_date',
    key: 'legal_maturity_date',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '主承销商',
    dataIndex: 'leder_under_writer',
    key: 'leder_under_writer',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '发行/管理人',
    dataIndex: 'issuer',
    key: 'issuer',
    render: (text: string) => commonUtils.formatContent(text, true)
  }, {
    title: '评级机构',
    dataIndex: 'rating_agency',
    key: 'rating_agency',
    render: (text: string) => commonUtils.formatContent(text, true)
  }
];