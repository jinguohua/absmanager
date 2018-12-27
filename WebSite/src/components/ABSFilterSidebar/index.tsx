import * as React from 'react';
import './index.less';
import FilterGroup from './filterGroup';

interface IProps {

}

class ABSFilterSidebar extends React.Component<IProps> {
  render() {
    return (
      <div className="filter-sidebar">
        <span>筛选条件</span>
        <FilterGroup title="产品主体" content={<div>test</div>}/>
        <FilterGroup title="发行管理人" content={<div>test</div>}/>
        <FilterGroup title="参与承销商" content={<div>test</div>}/>
        <FilterGroup title="市场分类" content={<div>test</div>}/>
        <FilterGroup title="产品分类" content={<div>test</div>}/>
        <FilterGroup title="发行时间" content={<div>test</div>}/>
      </div>
    );
  }
}

export default ABSFilterSidebar;