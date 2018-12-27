import React, { Component } from 'react';
import './index.less';
import { connect } from 'dva';
import { getTableTitle } from '../util';
import ABSList from '../../../../../components/ABSList';
import ABSPanel from '../../../../../components/ABSPanel';
import ABSLink from '../../../../../components/ABSLink';
import RouteConfig from '../../../../config/routeConfig';
export interface IDealInformationProps {
  dispatch: ({ }: any) => void;
  dealID: number;
}

export interface IDealInformationState {
}

class DealTable extends Component<IDealInformationProps, IDealInformationState> {

  getColumnsData = (text: string, record: any) => {
    const { note_id } = record;
    return <ABSLink to={`${RouteConfig.investmentSecurityInfo}${note_id}`} > {text}</ ABSLink>;
  }

  render() {
    const { dealID } = this.props;
    const columnsData = getTableTitle();
    columnsData[0].render = this.getColumnsData;
    return (
      <ABSPanel title="证券信息" removePadding={true}>
        <ABSList
          columnsData={columnsData}
          actionType="product/getStructureList"
          payload={{ deal_id: dealID }}
          model="product.structureList.note_list"
          scroll={{ x: 1024 }}
        />
      </ABSPanel>
    );
  }
}

const mapStateToProps = ({ global }) => {
  return {
    dealID: global.params.dealID,
  };
};

export default connect(mapStateToProps)(DealTable);