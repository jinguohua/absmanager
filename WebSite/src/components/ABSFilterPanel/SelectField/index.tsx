import React from 'react';
import Label from '../Label';
import { Select } from 'antd';
import './index.less';
import { IFilterSectionConfig, IFilterItemConfig } from '../interface';
import SelectDataHelper from '../utils/SelectDataHelper';

const Option = Select.Option;

export interface ISelectOption {
  key: number;
  value: string;
  selected: boolean;
}

export interface ISelectFieldProps {
  config: IFilterSectionConfig;
  onClick: (sectionConfig: IFilterSectionConfig, index: number) => void;
}

export interface ISelectFieldState {
  selectValue: string;
}

class SelectField extends React.PureComponent<ISelectFieldProps, ISelectFieldState> {
  constructor(props: ISelectFieldProps) {
    super(props);
    const { config } = props;
    let selectValue = '';
    if (config) {
      const data = config.value;
      selectValue = SelectDataHelper.getDefaultValue(data);
    }
    this.state = {
      selectValue,
    };
  }

  componentWillReceiveProps(nextProps: ISelectFieldProps) {
    const { config } = nextProps;
    if (!config) { return; }
    const data = config.value;
    const { selectValue } = this.state;
    const value = SelectDataHelper.getDefaultValue(data);
    if (value !== selectValue) {
      this.setState({ selectValue: value });
    }
  }

  handleChange = (value: string) => {
    //
  }

  handleBlur = () => {
    //
  }

  handleFocus = () => {
    //
  }

  filterOption = (input, option) => {
    return option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0;
  }

  onSelect = (value: any) => {
    this.setState({ selectValue: value });
    const { onClick, config } = this.props;
    onClick(config, value);
  }

  renderOptions(data: IFilterItemConfig[]) {
    if (!Array.isArray(data)) { return null; }
    return data.map((option, index, array) => {
      if (!option) { return null; }
      const value = option.value;
      return <Option key={`${index}`} value={index}>{value}</Option>;
    });
  }

  render() {
    const { config } = this.props;
    const { selectValue } = this.state;
    if (!config) { return null; }
    const title = config.title;
    const data = config.value;
    const placeholder = SelectDataHelper.getFirstElement(data);
    return (
      <div className="abs-filter-auto-complete">
        <Label title={title} />
        <Select
          className="abs-filter-auto-complete-select"
          showSearch={true}
          placeholder={placeholder}
          optionFilterProp="children"
          onChange={this.handleChange}
          onFocus={this.handleFocus}
          onBlur={this.handleBlur}
          filterOption={this.filterOption}
          onSelect={this.onSelect}
          dropdownClassName="abs-select-field-dropdown"
          value={selectValue}
        >
          {this.renderOptions(data)}
        </Select>
      </div>
    );
  }
}

export default SelectField;