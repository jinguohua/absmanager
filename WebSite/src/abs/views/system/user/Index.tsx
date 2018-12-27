import React from 'react';
import { Link } from 'dva/router';
import './Index.less';
import Container from '../../../../components/ABSFilterPanel/Container';
import ABSListContainer from '../../../../components/ABSListContainer';
// import { render } from 'react-dom';
import commonUtils from '../../../../utils/commonUtils';

const columnData = [
  {
    render: (text, record) => (
      <Link to={'../user/Edit' + record.id}>编辑</Link>
    ),
    title: (text, record) => <Link to={'../user/Create'}>新增</Link>
  },
  {
    title: '名称',
    dataIndex: 'userName',
    key: 'userName'
  },
  {
    title: '昵称',
    dataIndex: 'nickName',
    key: 'nickName'
  },
  {
    title: '电话',
    dataIndex: 'phoneNumber',
    key: 'phoneNumber',
    render: (text) => {
      return commonUtils.formatContent (text, false, false, false, 0);
    }
  },
  {
    title: '邮箱',
    dataIndex: 'email',
    key: 'emal'
  },
  {
    title: '角色',
    dataIndex: 'roles',
    key: 'roles'
  }
];

class UserList extends React.Component {
  handerFilter() {
    // alert('test');
  }

  render() {
    var filters = [];

    return (
      <div className="abs-user-list">
        <Container onConfirm={this.handerFilter}>
          <span>test</span>
        </Container>
        <div className="abs-list-right">
          <ABSListContainer
            actionType="system/getUserList"
            title="用户列表"
            model="system.userList"
            payload={filters}
            columnsData={columnData}
            bordered={true}
          />
        </div>
      </div>
    );
  }
}

export default UserList;
