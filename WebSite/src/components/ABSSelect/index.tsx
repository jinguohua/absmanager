import React from 'react';
import { Select, Spin } from 'antd';
import './index.less';
const Option = Select.Option;
import Request from '../../utils/http/request';
import { GlobalApi } from '../../abs/config/api';

export declare type selectSize = 'default' | 'large' | 'small';

export interface IABSSelectProps {
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  defaultValue?: string | number;
  value?: string | number | string[];
  size?: selectSize;
  onChange?: (value: any, option: any) => void;
  onSelect?: () => void;
  onSearch?: () => void;
  onFocus?: () => void;
  onBlur?: () => void;
  dropdownMatchSelectWidth?: boolean;
  dropdownClassName?: string;
  optionData?: any;
  mode?: 'multiple' | 'tags' | 'default';
  category?: string;
}

export class ABSSelect extends React.Component<IABSSelectProps, any> {
  select;

  constructor(props: IABSSelectProps) {
    super(props);

    const { optionData, value } = this.props;

    this.state = {
      optionData: optionData || [],
      value: value,
      isLoading: false
    };
  }

  selectOnSearch = (value: string) => {
    this.getDropdownData(value);
  }

  getDropdownData = (keywords: string) => {
    const { category = null } = this.props;
    if (category == null || category === '') {
      return;
    }

    const request = Request.get(GlobalApi.codeitemURL, {
      category: category,
      search: keywords
    });

    this.setState({
      ...this.state,
      isLoading: true,
      optionData: []
    });

    request.then(data => {
      const optionData = data.map(item => {
        return { title: item.value, value: item.key, labelName: item.value };
      });

      this.setState({
        ...this.state,
        optionData: optionData,
        isLoading: false
      });
    });
  }

  onChange = (value: any, option: any) => {
    const { onChange } = this.props;
    if (onChange) {
      onChange(value, option);
    }
    this.setState({ ...this.state, value: value });
    this.select.blur();
  }

  render() {
    const {
      size,
      placeholder,
      className,
      disabled,
      onSelect,
      onSearch,
      onFocus,
      onBlur,
      dropdownMatchSelectWidth,
      dropdownClassName,
      defaultValue,
      mode,
      category
    } = this.props;
    return (
      <div className="abs-select">
        <Select
          showSearch={!!category}
          defaultValue={defaultValue}
          value={this.state.value}
          className={className}
          size={size}
          placeholder={placeholder}
          disabled={disabled}
          onSelect={onSelect}
          onSearch={!!onSearch ? onSearch : this.selectOnSearch}
          onFocus={onFocus}
          onChange={this.onChange}
          onBlur={onBlur}
          dropdownMatchSelectWidth={dropdownMatchSelectWidth}
          dropdownClassName={dropdownClassName}
          mode={mode}
          ref={view => (this.select = view)}
          filterOption={!category}
          notFoundContent={this.state.isLoading ? <Spin /> : '没有数据'}
          allowClear={true}
        >
          {this.state.optionData.map((item, index) => {
            return (
              <Option
                key={`${item.value}${index}`}
                title={item.title}
                value={item.value}
              >
                {item.labelName}
              </Option>
            );
          })}
        </Select>
      </div>
    );
  }
}
