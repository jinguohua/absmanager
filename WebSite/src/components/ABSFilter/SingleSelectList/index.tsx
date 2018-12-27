import React from 'react';
import './index.less';
import { IFilterSectionConfig } from '../../ABSFilterPanel/interface';
import SingleSelect from '../SingleSelect';

export interface ISingleSelectListProps {
  config: IFilterSectionConfig;
  onClick: (sectionConfig: IFilterSectionConfig, index: number) => void;
}
 
export interface ISingleSelectListState {
  
}
 
class SingleSelectList extends React.Component<ISingleSelectListProps, ISingleSelectListState> {
  onClick = (itemIndex: number) => {
    const { onClick, config } = this.props;
    onClick(config, itemIndex);
  }

  render() { 
    const { config } = this.props;
    if (!config) {
      return null;
    }
    return (
      <div className="abs-filter-single-select-list">
        <div className="abs-filter-single-select-list-item">
          {this.renderCheckboxItems()}
        </div>
      </div>
    );
  }

  renderCheckboxItems() {
    const { config } = this.props;
    const data = config.value;
    return data.map((value, index, array) => {
      return <SingleSelect key={value.key} item={value} onClick={this.onClick} index={index} />;
    });
  }
}
 
export default SingleSelectList;