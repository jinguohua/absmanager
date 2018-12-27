import React from 'react';
import ABSFilterPanel from '../../../../components/ABSFilterPanel';
import { connect } from 'dva';
import { IFilterSectionConfig } from '../../../../components/ABSFilterPanel/interface';
import ABSListContainer from '../../../../components/ABSListContainer';
import ABSDownloadHeader from '../../../../components/ABSDownloadHeader';
import ABSLink from '../../../../components/ABSLink';
import '../../../../styles/coverAnt.less';
import './index.less';
import { securityColumnsData, productColumnsData } from './data';
import commonUtils from '../../../../utils/commonUtils';
import { FILTER_TABLE_NO_CONTENT_MESSAGE } from '../../../../utils/constant';
import routeConfig from '../../../config/routeConfig';

export interface IABSProductOrSecurityListProps {
  globalFilterList: any[];
  dispatch: ({ }: any) => void;
  type: string;
  dealSecurityList: any;
  dealProductList: any;
  globalDownload: string;
}

export interface IABSProductOrSecurityListState {
  list: any;
  pageIndex: number;
  filterQueryList: any;
  totalItems: number;
  totalCounts: number;
}

export function addSecurityInfoLink(text: any, record: any) {
  if (text === '') {
    return commonUtils.formatContent(text, true);
  }
  return (
    <ABSLink to={routeConfig.investmentSecurityInfo + record.security_id}>
      {commonUtils.formatContent(text, true)}
    </ABSLink>
  );
}

export function addLink(text: any, record: any) {
  if (text === '') {
    return commonUtils.formatContent(text, true);
  }
  return (
    <ABSLink to={routeConfig.productDealInfo + '?deal_id=' + record.deal_id}>
      {commonUtils.formatContent(text, true)}
    </ABSLink>
  );
}

class ABSProductOrSecurityListRoute extends React.PureComponent<IABSProductOrSecurityListProps, IABSProductOrSecurityListState> {
  private filterList: any = [];
  constructor(props: any) {
    super(props);
    this.filterList = commonUtils.getParams();
    const list = this.initData();
    this.state = {
      list: list,
      pageIndex: 1,
      filterQueryList: {
        page_size: 20,
        page_index: 1,
        filter_query_list: this.filterList.filter_query_list || [],
      },
      totalItems: 0,
      totalCounts: 0,
    };
  }

  componentDidMount() {
    const { list } = this.state;
    const payload = {
      default_selects: this.filterList.filter_query_list,
      filter_type_id: list.filter.payload.filter_type_id,
    };
    this.props.dispatch({ 
      type: 'product/getFilterLists', 
      payload: payload
    });
    this.updateListTotalData();
  }
  componentDidUpdate() {
    this.updateListTotalData();
  }

  initData() {
    const { type } = this.props;
    let thisList: any = {};
    if (type === 'product') {
      thisList = {
        table: {
          type: 'product/getDealProductList',
          columnsData: productColumnsData,
          modal: 'product.dealProductList'
        },
        filter: {
          payload: { filter_type_id: 1}
        },
        header: {
          title: '产品列表',
          type: 'product/getDealProductListExport',
        }
      };
    } else {
      thisList = {
        table: {
          type: 'product/getDealSecurityList',
          columnsData: securityColumnsData,
          modal: 'product.dealSecurityList'
        },
        filter: {
          payload: { filter_type_id: 2}
        },
        header: {
          title: '证券列表',
          type: 'product/getDealSecurityListExport',
        }
      };
    }
    return thisList;
  }  

  updateListTotalData() {
    const { dealSecurityList, dealProductList, type } = this.props;
    let dealList: any;
    if (type === 'product') {
      dealList = dealProductList;
    } else {
      dealList = dealSecurityList;
    }
    if (!dealList.extra_sums) {
      dealList.extra_sums = {
        total_count: 0,
        total_offering: 0
      };
    }
    this.setState({
      pageIndex: dealList.current_page || 1,
      totalItems: commonUtils.formatContent(dealList.extra_sums.total_count, false, false, false, 0),
      totalCounts: commonUtils.formatContent(dealList.extra_sums.total_offering, false, false, true, 2, 8),
    });
  }

  onClickItem = (value: IFilterSectionConfig[]) => {
    this.props.dispatch({ type: 'product/updateFilterList', payload: value });
  } 
   
  onConfirm = (value: any) => {
    const { pageIndex } = this.state; 
    const payload = {
      page_size: 20, 
      page_index: pageIndex,
      filter_query_list: value.filter_query_list
    };
    this.setState({
      filterQueryList: payload,
    });
  }

  onClose = (flag: boolean, value: IFilterSectionConfig[]) => {
    this.props.dispatch({ type: 'product/updateFilterList', payload: value });
  }

  render() {
    const { globalFilterList, dispatch } = this.props;
    const { list, filterQueryList, totalItems, totalCounts } = this.state; 
    const comment = `共 ${totalItems} 单；共 ${totalCounts} 亿`;
    return (
      <div className="abs-security-list">
        <ABSFilterPanel
          config={globalFilterList}
          onClickItem={this.onClickItem}
          onConfirm={this.onConfirm}
          onClose={this.onClose}
        />
        <div className="abs-list-right">
          <ABSDownloadHeader 
            title={list.header.title} 
            actionType={list.header.type} 
            payload={filterQueryList} 
            dispatch={dispatch}
          />
          <ABSListContainer
            title="相关信息"
            comment={comment}
            columnsData={list.table.columnsData}
            actionType={list.table.type}
            payload={filterQueryList}
            model={list.table.modal}
            removePerfectScrollBar={true}
            emptyText={FILTER_TABLE_NO_CONTENT_MESSAGE}
          />
        </div>
      </div>
    );
  }
}

function mapStateToProps({ product }: any) {
  return {
    globalFilterList: product.globalFilterList,
    dealSecurityList: product.dealSecurityList,
    dealProductList: product.dealProductList
  };
}

export default connect(mapStateToProps)(ABSProductOrSecurityListRoute);