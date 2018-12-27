import React from 'react';
import { Select, Spin, Row, Icon } from 'antd';
import './index.less';
import ISearchOption from './utils/ISearchOption';
import Span from '../ABSSpan';

const Option = Select.Option;

export interface IABSSearchFieldProps {
  title: string;
  placeholder?: string;
  defaultValue?: string;
  value: string;
  fetching: boolean;
  data: ISearchOption[];
  className?: string;
  disabled: boolean;
  fetchContent: (value: string) => void;
  handleChange: (value: number | string, option: any) => void;
}

export interface IABSSearchFieldState {

}

class ABSSearchField extends React.Component<IABSSearchFieldProps, IABSSearchFieldState> {
  renderOptions() {
    const { data } = this.props;
    return data.map((d, index: number) => (
      <Option
        key={`${d.value}`}
        title={d.text}
        value={d.value}
      >
        <Row>{d.text}</Row>
        <Row>{d.desc}</Row>
      </Option>
    ));
  }

  renderNotFoundContent = () => {
    const { fetching } = this.props;
    if (!fetching) { return null; }
    return <Spin size="small" />;
  }

  render() {
    const {
      title,
      placeholder,
      fetchContent,
      value,
      handleChange,
      className,
      disabled,
    } = this.props;
    return (
      <div className={`search-field ${className}`}>
        <Span className="search-field-title" required={true}>{title}</Span>
        <Select
          showSearch={true}
          value={value}
          placeholder={placeholder}
          notFoundContent={null}
          filterOption={false}
          onSearch={fetchContent}
          onChange={handleChange}
          style={{ width: '100%' }}
          disabled={disabled}
          suffixIcon={<Icon type="search" style={{ fontSize: '14px', color: '#999'}}/>}
        >
          {this.renderOptions()}
        </Select>
      </div>
    );
  }
}

export default ABSSearchField;