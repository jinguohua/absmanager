import React from 'react';
import './index.less';
import ABSFilter from '../../../../components/ABSFilter';
import { connect } from 'dva';
import { IFilterSectionConfig } from '../../../../components/ABSFilterPanel/interface';

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
  
  render() {
    const { filterData } = this.props;
    return (
      <div className="abs-filter-pannel-sample">
        <ABSFilter
          config={filterData}
          onClickItem={this.onClickItem}
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