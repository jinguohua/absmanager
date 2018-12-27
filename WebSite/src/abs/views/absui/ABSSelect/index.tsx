import React, { Component } from 'react';
import { ABSSelect } from '../../../../components/ABSSelect';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';
import './index.less';

export interface IAppProps {
}
export interface IAppState {
  selectVal?: string;
}

const selectOptions = [
  { value: '0', labelName: '全部', className: 'abs-select' },
  { value: '1', labelName: '正确', className: 'abs-select' },
  { value: '2', labelName: '错误', className: 'abs-select' },
  { value: '3', labelName: '未处理', className: 'abs-select' }
];
class ABSSelectRoute extends Component<IAppProps, IAppState> {
  selectChange = (value) => {  
    // 选中 option，或 input 的 value 变化（combobox 模式下）时，调用此函数 
  }

  selectSelect = () => { 
    // 被选中时调用
  }

  selectSearch = () => { 
    // 文本框值变化时回调
  }
  
  selectFocus = () => { 
    // 获得焦点时回调
  }

  selectBlur = () => { 
    // 失去焦点的时回调 
  }

  render () {
    return (
      <div className="absui-select">
        <ABSDescription>下拉框</ABSDescription>
        <ABSSelect 
          placeholder="请选择" 
          className=""  
          disabled={false} 
          defaultValue="默认值"
          size="default" 
          onChange={this.selectChange} 
          onSelect={this.selectSelect} 
          onSearch={this.selectSearch} 
          onFocus={this.selectFocus} 
          onBlur={this.selectBlur}
          dropdownMatchSelectWidth={true} 
          dropdownClassName=""
          optionData={selectOptions}
        />
      </div>
    );
  }
}

export default ABSSelectRoute;