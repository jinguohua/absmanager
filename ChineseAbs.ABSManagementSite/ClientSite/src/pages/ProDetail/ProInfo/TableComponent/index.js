import React from 'react';
import { Table } from 'antd';

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
    title: '角色',
    dataIndex: 'user',
    key: 'user',
  },
  {
    title: '机构名称',
    dataIndex: 'name',
    key: 'name',
  },
  {
    title: '',
    dataIndex: 'edit',
    key: 'edit',
    render: () => (
      <span>
        <a>Edit</a>&nbsp;&nbsp;&nbsp;
        <a>Delete</a>
      </span>
    ),
  },
];

function TableComponent() {
  return (
    <div>
      <h1>相关机构</h1>
      <Table dataSource={dataSource} columns={columns} />
    </div>
  );
}

export default TableComponent;
