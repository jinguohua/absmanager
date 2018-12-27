import { React, Form, alert } from 'react';
import { Pagination } from 'antd';
import ProjectList from './ProjectList';
// import { fetchProjects } from '../../services/project';

class ProductFilterable extends React.Component {
  hasUnmount = false;

  constructor(props) {
    super(props);
    this.state = {
      // list: [],
      // keyword: null,
      fetching: false,
      // total: 100,
      // category: '基金类',
      // currentPage: null,
      // pageSize: 5,
      // totalPage: null,

      // items: null,
    };
  }

  componentDidMount() {
    const { fetching } = this.state;
    if (!fetching) {
      const qs = {
        pageindex: 1,
        pagesize: this.state,
      };
      this.fetchData(qs);
    }
  }

  componentWillUnmount() {
    this.hasUnmount = true;
  }

  // fetchData = (para) => {
  //     this.setState({ ...this.state, fetching: true, items: [] });

  //     fetchProjects(para)
  //         .then(data => {
  //             if (!this.hasUnmount) {
  //                 const items = data.list || [];
  //                 const value = items.map(i => i.key);
  //                 this.setState({

  //                     fetching: false,
  //                     items,
  //                     value,
  //                     currentPage: data.pageIndex,
  //                     pageSize: data.pageSize,
  //                     totalPage: data.totalPage,
  //                     total: data.total,
  //                 });
  //             }
  //         }
  // );
  // }

  // 产品操作
  addProject = () => {
    alert('addProject...');
  };

  // 查询设置
  handleSearch = filters => {
    const { fetching } = this.state;
    if (!fetching) {
      const qs = {
        pageindex: 1,
        pagesize: this.state,
        filters,
      };
      this.fetchData(qs);
    }
  };

  handleClear = () => {
    alert('clear...');
  };

  // 分页设置
  onPageChange = pageNumber => {
    const { fetching } = this.state;
    if (!fetching) {
      const qs = {
        pageindex: pageNumber,
        pagesize: this.state,
      };
      this.fetchData(qs);
    }
  };

  render() {
    const { items, total, category, currentPage, pageSize } = this.props;

    const WrappedAdvancedSearchForm = Form.create()(this.ProjectSearch);
    return (
      <div>
        <WrappedAdvancedSearchForm handleSearchP={this.handleSearch} />
        <ProjectList
          items={items}
          total={total}
          category={category}
          onChange={() => this.onPageChange()}
        />
        <Pagination
          showQuickJumper
          total={total}
          onChange={this.onPageChange}
          pageSize={pageSize}
          current={currentPage}
          showTotal={`总数: ${total} `}
          showSizeChanger
          onShowSizeChange={this.onShowSizeChange}
        />
      </div>
    );
  }
}

export default ProductFilterable;
