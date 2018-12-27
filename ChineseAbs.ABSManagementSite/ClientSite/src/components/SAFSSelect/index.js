import { Select } from 'antd';
import React, { PureComponent } from 'react';
import { loadDataItem } from '../../services/api';

const { Option } = Select.Option;

class SAFSSelect extends PureComponent {
  state = {
    category: '',
    value: [],
    items: [],
    fetching: false,
  };

  hasUnmount = false;

  constructor(props) {
    super(props);
    const { category, value } = props;

    this.state.category = category;
    if (value != null) {
      this.state.fetching = true;
      let data = value;
      if (value.split) data = value.split(',');

      this.state.value = data;
    }
  }

  componentDidMount() {
    const { fetching, value } = this.state;
    if (fetching) {
      const values = value.join(',');
      this.fetchData(`=${values}`);
    }
  }

  componentWillUnmount() {
    this.hasUnmount = true;
  }

  fetchData = keywords => {
    const { category, value } = this.state;
    let newState = { ...this.state, fetching: true, items: [] };
    this.setState(newState);
    loadDataItem(category, keywords || '').then(data => {
      if (!this.hasUnmount) {
        const items = data || [];
        // if(this.state.value.length > 0){
        //     value = items.map(i=>i.key).find(x => value.indexOf(x) > -1);
        // }
        newState = { ...newState, fetching: false, items, value };
        this.setState(newState);
      }
    });
  };

  handleSearch = value => {
    this.fetchData(value);
  };

  handleChange = value => {
    this.setState({
      value,
      fetching: false,
    });
  };

  filterOptionHander = value => {
    console.log(`filter: ${value}`);
  };

  render() {
    const { category, notFoundContent, onSearch, showSearch, ...restprops } = this.props;
    const { fetching, items, value } = this.state;
    const pros = { mode: 'default', ...restprops };
    return (
      <Select
        {...pros}
        notFoundContent={null}
        showSearch
        allowClear
        defaultActiveFirstOption={false}
        value={value}
        onSearch={this.handleSearch}
        onChange={this.handleChange}
        tokenSeparators={[',']}
        filterOption={this.filterOption}
        loading={fetching}
      >
        {items.map(d => (
          <Option key={d.key}>{d.value}</Option>
        ))}
      </Select>
    );
  }
}

export default SAFSSelect;
