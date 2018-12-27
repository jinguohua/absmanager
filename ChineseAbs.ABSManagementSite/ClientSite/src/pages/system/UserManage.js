import React from 'react';
import { Table } from 'antd';
// import EditUserAction from './Edit';
import RePwdButton from './ResetPassword';
// import DialogForm from './Dialog'
import { GetUserList, DeleteUser } from '../../services/api';

let result = [];

function Delete(record) {
  const { id } = record.id;
  DeleteUser({ id }).then();
}

const columns = [
  {
    title: '用户名',
    dataIndex: 'userName',
    key: 'userName',
    render: text => <a href="#">{text}</a>,
  },
  {
    title: '昵称',
    dataIndex: 'nickName',
    key: 'nickName',
  },
  {
    title: '电话',
    dataIndex: 'phoneNumber',
    key: 'phoneNumber',
  },
  {
    title: '邮箱',
    dataIndex: 'email',
    key: 'email',
  },
  {
    title: '角色',
    key: 'roles',
    dataIndex: 'roles',
    // render: tags => (
    //   <span>
    //     {tags.map(tag => <Tag color="blue" key={tag}>{tag}</Tag>)}
    //   </span>
    // ),
  },
  {
    title: '操作',
    key: 'action',
    render: (text, record) => (
      <span style={{ display: 'flex', 'justify-content': 'space-around' }}>
        <RePwdButton row={record} />
        {/* <EditUserAction row={record}  />   */}
        {/* <DialogForm row={record} /> */}
        <a href="#" onClick={() => Delete(record)}>
          删除
        </a>
      </span>
    ),
  },
];

function UserIndex() {
  GetUserList().then(data => {
    result = data;
  });
  return (
    <div>
      <Table columns={columns} dataSource={result} />
    </div>
  );
}

export default UserIndex;
