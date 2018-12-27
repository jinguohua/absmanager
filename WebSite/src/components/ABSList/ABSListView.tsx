import React from 'react';
import { Table } from 'antd';
import './index.less';
import CommonUtils from '../../utils/commonUtils';
import ABSLoading from '../../components/ABSLoading';
import classNames from 'classnames';

export interface IABSListProps {
  type?: string;
  tableWidth?: string;
  bordered?: boolean;
  columnsData?: any;
  contentData?: any;
  onChange?: any;
  loading?: boolean;
  scroll?: any;
  emptyText?: string;
  onRow?: (record: any, index: number) => any;
  // 给定一些默认参数
  paginationData?: {
    current?: number,
    defaultCurrent?: number,
    defaultPageSize?: number,
    hideOnSinglePage?: boolean,
    total?: number,
    pageSize?: number,
  } | false;
  expandedRowKeys?: any[];
  onExpand?: (expanded: any, record: any) => any;
  rowClassName?: (record: any, index: number) => any;
}

export default class ABSListView extends React.Component<IABSListProps, any> {

  public static defaultProps = {
    current: 1,
    defaultCurrent: 1,
    defaultPageSize: 10,
    hideOnSinglePage: false,
    total: 10,
    pageSize: 5,
    emptyText: '暂无数据',
    expandedRowKeys: [],
  };

  render() {
    const { scroll, type, tableWidth, bordered, columnsData, contentData, paginationData, onChange, loading, emptyText, onRow, expandedRowKeys, onExpand, rowClassName } = this.props;
    const tableStyle = {
      width: tableWidth,
    };
    const columnData = columnsData ? columnsData : [];

    const myColumnsData = columnData.map((column) => {
      if (column.render && typeof column.render === 'function') {
        return column;
      }
      return {
        ...column,
        render: (text) => {
          return CommonUtils.formatContent(text, true, true);
        },
      };
    });
    // 判断是否有传递 来决定是否显示页数选择器
    const pagination = paginationData ? paginationData : false;
    const classes = classNames('abs-table', {
      [`abs-table-${type}`]: type,
    });
    return (
      <div className={classes} style={tableStyle}>
        <Table
          bordered={bordered}
          columns={myColumnsData}
          dataSource={contentData}
          pagination={pagination}
          onChange={onChange}
          scroll={scroll}
          loading={{ indicator: <ABSLoading color="blue" />, spinning: loading }}
          locale={{ emptyText }}
          onRow={onRow}
          expandedRowKeys={expandedRowKeys}
          onExpand={onExpand}
          rowClassName={rowClassName}
        />
      </div>
    );

  }
}
