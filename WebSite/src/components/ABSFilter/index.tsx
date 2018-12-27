import React from 'react';
import { IFilterSectionConfig } from '../ABSFilterPanel/interface';
import { updateFilterDataState } from './util';
import SingleSelectList from './SingleSelectList';

export interface IABSFilterProps {
  config: IFilterSectionConfig[];
  onClickItem: (value: IFilterSectionConfig[]) => void;
}
 
export interface IABSFilterState {
  
}
 
class ABSFilter extends React.PureComponent<IABSFilterProps, IABSFilterState> {

  onClickItem = (sectionConfig: IFilterSectionConfig, itemIndex: number) => {
    if (!sectionConfig) {
      return;
    }
    const { config, onClickItem } = this.props;
    const sectionKey = sectionConfig.key;
    const updatedData = updateFilterDataState(config, sectionKey, itemIndex);
    onClickItem(updatedData);
  }

  render() { 
    return (
      <div>  
        {this.renderContent()}
      </div>
    );
  }

  renderContent() {
    const { config } = this.props;
    if (!Array.isArray(config)) {
      return null;
    }
    return config.map((value, index, array) => {
      if (!value) {
        return null;
      }
      if (value.value === undefined) {
        return null;
      }

      return <SingleSelectList key={index} config={value} onClick={this.onClickItem} />;
    });
  }
}
 
export default ABSFilter;