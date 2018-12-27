import React from 'react';
import { AutoComplete, Input, Icon } from 'antd';
import './index.less';

export interface IABSAutoCompleteProps {
  dataSource: Array<any>;
  placeholder?: string;
  onSearch?: any;
  onChange?: any;
}

class ABSAutoComplete extends React.Component<IABSAutoCompleteProps> {

  static defaultProps = {
    placeholder: '输入名称、拼音，搜索产品',
  };

  render() { 
    const { dataSource, onSearch, onChange, placeholder } = this.props;
    return (
      <div className="abs-auto-complete">
        <AutoComplete
          dropdownMatchSelectWidth={false}
          dropdownStyle={{ width: 300 }}
          dropdownClassName="abs-auto-complete-dropdown"
          size="large"
          style={{ width: '550px' }}
          dataSource={dataSource}
          optionLabelProp="value"
          onSearch={onSearch}
          onChange={onChange}
        >
          <Input suffix={<Icon type="search" className="abs-auto-complete-icon" />} placeholder={placeholder} />
        </AutoComplete>
      </div>
    );
  }
}
 
export default ABSAutoComplete;