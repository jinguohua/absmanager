import React from 'react';
import { Link } from 'dva/router';
import './index.less';
import Container from '../../../../components/ABSFilterPanel/Container';
import ABSListContainer from '../../../../components/ABSListContainer';
import { ABSSelect } from '../../../../components/ABSSelect';

const columnData = [
  {
    render: (text, record) => {
      return <Link to={'../project/base?id=' + record.id}>编辑</Link>;
    },
    title: (text, record) => <Link to={'../project/base?id=0'}>新增</Link>
  },
  {
    title: '产品名称',
    dataIndex: 'name',
    key: 'name'
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status'
  },
  {
    title: '产品类型',
    dataIndex: 'projectType',
    key: 'projectType'
  },
  {
    title: '交易所',
    dataIndex: 'Exchange',
    key: 'Exchange'
  },
  {
    title: '发行金额',
    dataIndex: 'TotalOffering',
    key: 'TotalOffering'
  }
];

class ProjectList extends React.Component {
  handerFilter() {
    alert('test');
  }

  render() {
    var filters = [];

    return (
      <div className="abs-project-list">
        <Container onConfirm={this.handerFilter}>
          <ABSSelect category="user" mode="multiple" />
        </Container>
        <div className="abs-list-right">
          <ABSListContainer
            actionType="project/getProjectList"
            title="项目列表"
            model="project.projectList"
            payload={filters}
            columnsData={columnData}
            bordered={true}
          />
        </div>
      </div>
    );
  }
}

export default ProjectList;
