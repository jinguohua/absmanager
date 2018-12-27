import React from 'react';
import { connect } from 'dva';
import { Link } from 'dva/router';
import ABSListContainer from '../../../../../components/ABSListContainer';
import ABSFilterPanel from '../../../../../components/ABSFilterPanel';
import { IFilterSectionConfig } from '../../../../../components/ABSFilterPanel/interface';
import ABSProgress from '../../../../../components/ABSProgress';
import { FILTER_TABLE_NO_CONTENT_MESSAGE } from '../../../../../utils/constant';
import commonUtils from '../../../../../utils/commonUtils';
import ABSDownloadHeader from '../../../../../components/ABSDownloadHeader';
import { IProgressNode } from '../../../../../components/ABSProgress/ABSProgressItem';
import { dealListIssuingColumnsData } from './data';
import './index.less';
import { FilterPanelType } from '../../../../../components/ABSFilterPanel/enum';

const FILTER_TYPE_SECURITY = 3;
export interface IDealListIssuingProps {
  globalFilterList: any[];
  dispatch: ({ }: any) => void;
  type: string;
  dealIssuingList: any;
  dealPipelineScheduleList: IProgressNode[];
  dealID: any;
  onRow?: (record: any, index: number) => any;
}
export interface IDealListIssuingState {
  filterQueryList: any;
  isShowDetail: boolean;
  isAbn: boolean;
  detailTitle: string;
}
export function addLink(text: any, record: any) {
  if (text === '') {
    return commonUtils.formatContent(text, true);
  }
  return (
    <Link to={'../detail/basic-info?id=' + record.id} >
      {commonUtils.formatContent(text, true)}
    </Link>
  );
}
class DealListIssuing extends React.Component<IDealListIssuingProps, IDealListIssuingState> {

  private filterList: any;

  constructor(props: any) {
    super(props);
    this.filterList = commonUtils.getParams();
    this.state = {
      filterQueryList: {
        page_size: 20,
        page_index: 1,
        filter_query_list: this.filterList.filter_query_list || []
      },
      isShowDetail: false,
      isAbn: false,
      detailTitle: '',
    };
  }

  componentDidMount() {
    const { dispatch } = this.props;
    dispatch({
      type: 'product/getFilterList',
      payload: { filter_type_id: FILTER_TYPE_SECURITY, default_selects: this.filterList.filter_query_list || [] },
    });
  }

  onClickFilterItem = (value: IFilterSectionConfig[]) => {
    this.props.dispatch({ type: 'product/updateFilterList', payload: value });
  }

  onConfirm = (value: any) => {
    const { dealIssuingList } = this.props;
    const payload = {
      page_size: 20,
      page_index: dealIssuingList.current_page,
      filter_query_list: value.filter_query_list
    };
    this.setState({
      filterQueryList: payload,
    });
  }

  onClose = (flag: boolean, value: IFilterSectionConfig[]) => {
    this.props.dispatch({ type: 'product/updateFilterList', payload: value });
  }

  onRow = (record: any, index: number) => {
    const { dispatch, dealID } = this.props;
    const { isAbn } = this.state;
    return {
      onClick: () => {
        if (!dealID) {
          this.setState({
            isShowDetail: false,
          });
        }
        this.setState({
          isShowDetail: true,
          detailTitle: record.full_name,
        });
        if (record.full_name.indexOf('ABN') > 0 || record.full_name.indexOf('abn') > 0) {
          this.setState({
            isAbn: true,
          });
        } else {
          this.setState({
            isAbn: false,
          });
        }
        dispatch({
          type: 'product/getDealPipelineScheduleList',
          payload: {
            deal_id: record.id,
            asset_type: record.deal_type,
            is_ABN: isAbn,
          }
        });
      }
    };
  }

  defaultDetail() {
    return (
      <div className="deal-list-issuing-default-title">
        请点击产品所在行，查看产品发行流程图
      </div>
    );
  }

  specificDetails() {
    const { dealPipelineScheduleList } = this.props;
    const { detailTitle } = this.state;
    return (
      <div className="deal-list-issuing-special">
        <div className="deal-list-issuing-title">
          <p>{detailTitle}</p>
        </div>
        <ABSProgress className="absui-progress" dataSource={dealPipelineScheduleList} />
      </div>
    );
  }

  isShowDetail() {
    const { isShowDetail } = this.state;
    if (isShowDetail) {
      return this.specificDetails();
    } else {
      return this.defaultDetail();
    }
  }
  onChange = () => {
    this.setState({
      isShowDetail: false
    });
    this.isShowDetail();
  }

  renderLeft() {
    const { globalFilterList } = this.props;
    return (
      <div className="deal-list-issuing-left">
        <ABSFilterPanel
          config={globalFilterList}
          onClickItem={this.onClickFilterItem}
          onConfirm={this.onConfirm}
          type={FilterPanelType.product}
          onClose={this.onClose}
        />
      </div>
    );
  }

  renderRight() {
    const { dispatch, dealIssuingList } = this.props;
    const newDealIssuingList = dealIssuingList ? dealIssuingList : [];
    if (!newDealIssuingList.extra_sums) {
      newDealIssuingList.extra_sums = {
        total_count: 0,
        total_expected_offering: 0,
        total_offering: 0,
      };
    }
    const { filterQueryList } = this.state;
    const totalCount = commonUtils.formatContent(newDealIssuingList.extra_sums.total_count, false, false, true, 0, 0);
    const totalExpectedOffering = commonUtils.formatContent(newDealIssuingList.extra_sums.total_expected_offering, false, false, true, 2, 8);
    const totalOffering = commonUtils.formatContent(newDealIssuingList.extra_sums.total_offering, false, false, true, 2, 8);
    const comment = `共${totalCount}单；拟发行总额${totalExpectedOffering}亿；发行总额${totalOffering}亿`;

    return (
      <div className="deal-list-issuing-right">
        <ABSDownloadHeader
          title="过会产品"
          actionType="product/getDealPipelineExport"
          payload={filterQueryList}
          dispatch={dispatch}
        />
        <div className="deal-list-issuing-filter">
          <div className="deal-list-issuing-filter-table">
            <ABSListContainer
              selectable={true}
              title="过会列表"
              emptyText={FILTER_TABLE_NO_CONTENT_MESSAGE}
              model="product.dealIssuingList"
              comment={comment}
              actionType="product/getDealIssuingList"
              columnsData={dealListIssuingColumnsData}
              payload={filterQueryList}
              removePerfectScrollBar={true}
              onRow={this.onRow}
              onChange={this.onChange}
            />
          </div>
          <div className="deal-list-issuing-detail">
            {
              this.isShowDetail()
            }
          </div>
        </div>
      </div>
    );
  }

  render() {
    return (
      <div className="deal-list-issuing">
        {this.renderLeft()}
        {this.renderRight()}
      </div>
    );
  }
}

function mapStateToProps({ product, global }: any) {
  return {
    globalFilterList: product.globalFilterList,
    dealIssuingList: product.dealIssuingList,
    dealPipelineScheduleList: product.dealPipelineScheduleList,
    dealID: global.params.dealID,
  };
}

export default connect(mapStateToProps)(DealListIssuing);