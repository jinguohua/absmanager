import React from 'react';
import ABSSearchBar from '../../../../components/ABSSearchBar';
import ABSDescription from '../../../../components/ABSDescription';
import ABSAutoComplete from '../../../../components/ABSAutoComplete';
import { AutoComplete } from 'antd';
import { connect } from 'dva';

const Option = AutoComplete.Option;

interface IABSUISearchBarState {
  dataSource: any;
}

class ABSUISearchBar extends React.Component<any, IABSUISearchBarState> {
  constructor(props: any) {
    super(props);
    this.state = {
      dataSource: []
    };
  }

  handleSearch = (value) => {
    this.props.dispatch({
      type: 'global/search',
      payload: {
        keyword: value,
        search_type: 1,
      },
    }).then((response) => {
      if (response) {
        this.setState({ dataSource: response });
      }
    });
  }

  onChange = (value) => {
    // const { dataSource } = this.state;
    // const selectedRow = dataSource.filter((row) => row.title === value);
  }

  renderOption(item: any) {
    return (
      <Option key={item.title} title={item.title}>
        {item.title}
      </Option>
    );
  }

  render() {
    const { dataSource } = this.state;
    return (
      <div className="absui-search-bar">
        <ABSDescription>搜索框样式1：用在头部 用于全局搜索</ABSDescription>
        <ABSSearchBar size="small" style={{ marginBottom: 20, marginTop: 20 }} />
        <ABSDescription style={{ marginTop: 32 }}>搜索框样式2：用在首页</ABSDescription>
        <ABSSearchBar size="large" />
        <ABSDescription style={{ marginTop: 32 }}>搜索框样式3：自动完成</ABSDescription>
        <ABSAutoComplete dataSource={dataSource.map(this.renderOption)} onSearch={this.handleSearch} onChange={this.onChange} />
      </div>
    );
  }
}

const mapStateToProps = ({ global }) => {
  return { ...global };
};

export default connect(mapStateToProps)(ABSUISearchBar);