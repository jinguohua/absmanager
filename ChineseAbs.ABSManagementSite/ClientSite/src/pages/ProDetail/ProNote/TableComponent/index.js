import React from 'react';
import { Table } from 'antd';
import ModelComponent from '../ModelComponent/index';

const dataSource = [
  {
    key: '1',
    user: '发起机构',
    name: '金融股份有限公司',
    edit: '西湖区湖底公园1号',
  },
  {
    key: '2',
    user: '发行人',
    name: '中信信托有限责任公司',
    edit: '西湖区湖底公园1号',
  },
];

const columns = [
  {
    title: '名称',
    dataIndex: 'user',
    key: 'user',
  },
  {
    title: '偿付类型',
    dataIndex: 'name',
    key: 'a',
  },
  {
    title: '本金占比',
    dataIndex: 'name',
    key: 'b',
  },
  {
    title: '期限',
    dataIndex: 'name',
    key: 'c',
  },
  {
    title: '利率',
    dataIndex: 'name',
    key: 'd',
  },
  {
    title: '',
    dataIndex: 'edit',
    key: 'edit',
    render: () => (
      <span>
        <ModelComponent dataSource={dataSource} />
        &nbsp;&nbsp;&nbsp;
        <a>Delete</a>
      </span>
    ),
  },
];

function TableComponent() {
  return (
    <div>
      <Table dataSource={dataSource} columns={columns} />
    </div>
  );
}

export default TableComponent;
