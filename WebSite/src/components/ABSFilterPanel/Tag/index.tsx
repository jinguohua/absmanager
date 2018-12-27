import React from 'react';
import './index.less';
import classNames from 'classnames';
import { IFilterItemConfig } from '../interface';

export interface ITagProps {
  item: IFilterItemConfig;
  index: number;
  onClick: (index: number) => void;
}
 
export interface ITagState {
  
}
 
class Tag extends React.Component<ITagProps, ITagState> {
  onClick = () => {
    const { onClick, index } = this.props;
    onClick(index);
  } 

  render() { 
    const { item } = this.props;
    if (!item) {
      return null;
    }
    const hide = item.hide;
    if (hide) {
      return null;
    }
    const title = item.value;
    const selected = item.selected;
    const style = classNames('abs-multi-select', { 'abs-multi-select-selected': selected });
    return (
      <div className={style} onClick={this.onClick}>
        {title}
      </div>
    );
  }
}
 
export default Tag;