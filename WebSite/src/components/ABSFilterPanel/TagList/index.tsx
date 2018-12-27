import React from 'react';
import Label from '../Label';
import Tag from '../Tag';
import './index.less';
import { IFilterSectionConfig } from '../interface';

export interface ITagListProps {
  config: IFilterSectionConfig;
  onClick: (sectionConfig: IFilterSectionConfig, index: number) => void;
}
 
export interface ITagListState {
  
}
 
class TagList extends React.Component<ITagListProps, ITagListState> {
  onClick = (itemIndex: number) => {
    const { onClick, config } = this.props;
    onClick(config, itemIndex);
  }

  render() { 
    const { config } = this.props;
    if (!config) {
      return null;
    }
    const title = config.title;
    return (
      <div className="abs-filter-multi-select-list">
        <Label title={title} />
        <div className="abs-filter-multi-select-list-item">
          {this.renderCheckboxItems()}
        </div>
      </div>
    );
  }

  renderCheckboxItems() {
    const { config } = this.props;
    const data = config.value;
    return data.map((value, index, array) => {
      return <Tag key={value.key} item={value} onClick={this.onClick} index={index} />;
    });
  }
}
 
export default TagList;