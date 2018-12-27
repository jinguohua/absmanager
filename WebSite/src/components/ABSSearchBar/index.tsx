import React from 'react';
import { Input } from 'antd';
import './index.less';

const Search = Input.Search;

export interface IABSSearchBarProps {
  value?: string;
  size: 'small' | 'large';
  style?: React.CSSProperties;
  onSearch?: (activeKey: string) => void;
}

export interface IABSSearchBarState {

}

class ABSSearchBar extends React.Component<IABSSearchBarProps, IABSSearchBarState> {
  render() {
    const { size, style, onSearch, value } = this.props;

    if (size === 'small') {
      return (
        <div className="abs-search-bar-small">
          <Search
            value={value}
            placeholder="证券、产品、机构、专家..."
            onSearch={onSearch}
            style={{ width: 220, ...style }}
          />
        </div>
      );
    }
    return (
      <div className="abs-search-bar-large">
        <Search
          defaultValue={value}
          placeholder="输入名称、拼音或代码，搜索产品、证券、机构和专家"
          onSearch={onSearch}
          style={{ width: 640, ...style }}
        />
      </div>
    );
  }
}

export default ABSSearchBar;