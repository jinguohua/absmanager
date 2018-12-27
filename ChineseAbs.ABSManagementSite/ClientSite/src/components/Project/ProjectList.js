import { React, console, alert } from 'react';
import './ProjectList.css';
import { Table, Divider, Popconfirm, message } from 'antd';

function DeleteComfirm(props) {
  const { id } = props;
  function confirm(e) {
    console.log(e);
    message.success(`Click on Yes.id:${id}`);
  }

  function cancel(e) {
    console.log(e);
    message.error(`Click on Cancle id:${id}`);
  }

  return (
    <Popconfirm
      title="确定要删除吗"
      onConfirm={confirm}
      onCancel={cancel}
      okText="确认"
      cancelText="取消"
    >
      <a href="#">删除</a>
    </Popconfirm>
  );
}

function MyTable(prop) {
  // const {Column, ColumnGroup} = Table;
  const data = prop.items;

  function handleEdit(id) {
    // this.preventDefault();
    alert(`编辑id:${id}`);
  }

  // function handleDelete (id) {
  //   alert (`删除id:${id}`);
  // }

  const columns = [
    {
      title: '项目简称',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
    },
    {
      title: '类型',
      dataIndex: 'projectType',
      key: 'projectType',
    },
    {
      title: '发行总额',
      dataIndex: 'totalOffering',
      key: 'totalOffering',
    },
    {
      title: '资产总额',
      dataIndex: 'totalOffering',
      key: 'id',
    },
    {
      title: '发行时间',
      dataIndex: 'issueDate',
      key: 'id',
    },
    {
      title: '期限',
      dataIndex: 'limit',
      key: 'id',
    },
    {
      title: 'Action',
      key: 'action',
      render: (text, record) => (
        <span>
          <a href="#" onClick={() => handleEdit(record.id)}>
            编辑
          </a>
          <Divider type="vertical" />
          <DeleteComfirm id={record.id} />
        </span>
      ),
    },
  ];

  return (
    <div>
      <Table columns={columns} dataSource={data} pagination={false} />
    </div>
  );
}

class ProjectList extends React.Component {
  hasUnmount = false;

  constructor(props) {
    super(props);
    this.state = {
      fetching: false,
    };
  }

  componentDidMount() {
    const { fetching } = this.state;
    if (!fetching) {
      const qs = {
        pageindex: 1,
        pagesize: 2,
      };
      console.log(qs);
    }
  }

  componentWillUnmount() {
    this.hasUnmount = true;
  }

  onShowSizeChange(current, pageSize) {
    this.console.log(current, pageSize);
  }

  handlePageChange = pageNumber => {
    this.onPageChange(pageNumber);
  };

  handleAdd = () => {
    alert('add...');
  };

  render() {
    const { items } = this.props;
    return (
      <div>
        {/* <Button type="primary" onClick={this.handleAdd} >新增</Button> */}
        <MyTable items={items} />
      </div>
    );
  }
}

export default ProjectList;
