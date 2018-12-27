import React from 'react';
import './index.less';
import ABSFilterPanel from '../../../../components/ABSFilterPanel';
import { connect } from 'dva';
import { IFilterSectionConfig } from '../../../../components/ABSFilterPanel/interface';
import { FilterPanelType } from '../../../../components/ABSFilterPanel/enum';

const data = require('./data.json');

export interface IABSFilterPanelSampleProps {
  filterData: any[];
  dispatch: any;
}
 
export interface IABSFilterPanelSampleState {
  
}
 
class ABSFilterPanelSample extends React.PureComponent<IABSFilterPanelSampleProps, IABSFilterPanelSampleState> {
  componentDidMount() {
    this.props.dispatch({ type: 'absui/updateFilterData', payload: data });
  }

  onClickItem = (value: IFilterSectionConfig[]) => {
    this.props.dispatch({ type: 'absui/updateFilterData', payload: value });
  }

  onConfirm = (value: any) => {
    // console.log('selected value: ', value);
  }

  onClose = (flag: boolean, value: any) => {
    //
  }
  
  render() {
    const { filterData } = this.props;
    return (
      <div className="abs-filter-pannel-sample">
        <ABSFilterPanel
          config={filterData}
          onClickItem={this.onClickItem}
          onConfirm={this.onConfirm}
          onClose={this.onClose}
          type={FilterPanelType.organization}
        />
        <div className="abs-filter-pannel-sample-right" />
      </div>
    );
  }
}

function mapStateToProps({ absui }: any) {
  const { filterData } = absui;
  return {
    filterData,
  };
}
 
export default connect(mapStateToProps)(ABSFilterPanelSample);