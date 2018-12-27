import React from 'react';
import { Pagination } from 'antd';
import classNames from 'classnames';
import './ABSPaginationView.less';

export interface IABSPaginationViewProps {
  defaultCurrent?: number;
  defaultPageSize?: number;
  total?: number;
  pageSize?: number;
  current?: number;
  onChange?: any;

  className?: string;
  style?: React.CSSProperties;
  hideOnSinglePage?: boolean;
}
 
class ABSPaginationView extends React.Component<IABSPaginationViewProps> {

  static defaultProps = {
    defaultCurrent: 1,
    defaultPageSize: 10,
    pageSize: 10,
    current: 1,
    total: 10,
  };
  
  render() { 
    const { defaultCurrent, defaultPageSize, total, current, pageSize, onChange, className, style, hideOnSinglePage } = this.props;
    const classes = classNames('abs-pagination', className);
    return (
      <Pagination 
        className={classes}
        defaultCurrent={defaultCurrent} 
        defaultPageSize={defaultPageSize}
        total={total} 
        current={current}
        pageSize={pageSize}
        onChange={onChange}
        style={style}
        hideOnSinglePage={hideOnSinglePage}
      />
    );
  }
}
 
export default ABSPaginationView;