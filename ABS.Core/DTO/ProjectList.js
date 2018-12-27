import React from 'react';
import projectcss from './ProjectList.css'
import { Table, Divider, Tag } from 'antd';
import { func } from 'prop-types';
import { Popconfirm, message } from 'antd';


function DeleteComfirm(props){
    const id = props.id;
    function confirm(e) {
        console.log(e);
        message.success('Click on Yes.id:'+id);
      }
      
      function cancel(e) {
        console.log(e);
        message.error('Click on No.id:'+id);
      }

      return (
        <Popconfirm title="确定要删除吗" onConfirm={confirm} onCancel={cancel} okText="确认" cancelText="取消">
          <a href="javascript:void(0)">删除</a>
        </Popconfirm>
      );
}


function MyTable(prop) {

    const { Column, ColumnGroup } = Table;
    const data = prop.items;

    function handleEdit(id) {
        //this.preventDefault();
        alert("编辑id:" + id);
    };

    function handleDelete(id){
        alert("删除id:" + id);
    }

    return (
        <div>
            <Table dataSource={data}>
                <Column
                    title="ID"
                    dataIndex="id"
                    key="id"
                />
                <Column
                    title="项目简称"
                    dataIndex="firstName"
                    key="firstName"
                />
                <Column
                    title="状态"
                    dataIndex="lastName"
                    key="lastName"
                />
                <Column
                    title="类型"
                    dataIndex="age"
                    key="age"
                />
                <Column
                    title="发行总额"
                    dataIndex="address"
                    key="address"
                />
                <Column
                    title="资产总额"
                    dataIndex="tags"
                    key="tags"
                    // render={tags => (
                    //     <span>
                    //         {tags.map(tag => <Tag color="blue" key={tag}>{tag}</Tag>)}
                    //     </span>
                    // )}
                />
                  <Column
                    title="发行时间"
                    dataIndex="address"
                    key="address"
                />
                <Column
                    title="操作"
                    key="action"
                    render={(text, record) => (
                        <span>
                            <a href="javascript:void(0)" onClick={()=>handleEdit(record.id)} >编辑</a>
                            <Divider type="vertical" />
                            <DeleteComfirm id={record.id}/>
                            {/* <a href="javascript:void(0)" onClick={()=>handleDelete(record.id)}>删除</a> */}
                        </span>
                    )}
                />
            </Table>
        </div>

    );
}

class ProjectList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isToggleOn: true,
        };
    }

    render() {
        return (
            <div>
                <span>Total:{this.props.items.length} </span>
                <h2> Category:{this.props.category}</h2>
                <MyTable items={this.props.items} />
            </div>
        );
    }
}

export default ProjectList;