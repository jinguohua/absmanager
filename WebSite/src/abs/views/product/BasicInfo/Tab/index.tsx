import React from 'react';
import { connect } from 'dva';
import './index.less';
import { ABSTab } from '../../../../../components/ABSTab';
import { IProgressNode } from '../../../../../components/ABSProgress/ABSProgressItem';
import Organization from './Organization';
import Cycle from './Cycle';
import { IOrganizationListItem } from './Organization/Item';

export interface ITabProps {
  productCycleList: IProgressNode[];
  dispatch: any;
  organizationList: IOrganizationListItem[];
  activeTabKey: string;
}

export interface ITabState {

}

class Tab extends React.PureComponent<ITabProps, ITabState> {
  onChangeTab = (key: string) => {
    this.props.dispatch({
      type: 'product/changeTab',
      payload: key,
    });
  }  

  render() {
    const {
      productCycleList,
      organizationList,
      activeTabKey,
    } = this.props;
    const panesList = [
      {
        title: '产品周期',
        key: '1',
        content: <Cycle cycles={productCycleList} />,
      },
      {
        title: '参与机构',
        key: '3',
        content: <Organization organizations={organizationList} />,
      },
    ];
    return (
      <div className="product-extras">
        <ABSTab
          activeKey={activeTabKey}
          panesList={panesList}
          onChange={this.onChangeTab}
          className="product-extras-tab"
          type="product"
        />
      </div>
    );
  }
}

function mapStateToProps({ product }: any) {
  const {
    productCycleList,
    expertList,
    organizationList,
    activeTabKey,
  } = product;
  return {
    productCycleList,
    expertList,
    organizationList,
    activeTabKey,
  };
}

export default connect(mapStateToProps)(Tab);