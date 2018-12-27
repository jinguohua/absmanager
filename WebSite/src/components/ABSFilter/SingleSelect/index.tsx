import React from 'react';
import './index.less';
import classNames from 'classnames';
import { IFilterItemConfig } from '../../ABSFilterPanel/interface';

export interface ISingleSelectProps {
  item: IFilterItemConfig;
  index: number;
  onClick: (index: number) => void;
}
 
export interface ISingleSelectState {
  
}
 
class SingleSelect extends React.Component<ISingleSelectProps, ISingleSelectState> {
  onClick = () => {
    const { onClick, index } = this.props;
    onClick(index);
  } 

  render() { 
    const { item } = this.props;
    if (!item) {
      return null;
    }
    const title = item.value;
    const selected = item.selected;
    const style = classNames('abs-single-select', { 'abs-single-select-selected': selected });
    return (
      <div className={style} onClick={this.onClick}>
        {title}
      </div>
    );
  }
}
 
export default SingleSelect;