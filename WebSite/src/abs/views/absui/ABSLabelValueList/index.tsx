import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSLabelValueList from '../../../../components/ABSLabelValueList';
import ABSEmphasizeText from '../../../../components/ABSEmphasizeText';
import ABSDescription from '../../../../components/ABSDescription';

class ABSUILabelValueList extends Component {
  render () {
    const list = [
      {
        title: '产品简称',
        content: '丰耀2017-2',
      },
      {
        title: '总金额（亿）',
        content: <ABSEmphasizeText title="30" style={{ color: '#FF1515'}} />,
      },
      {
        title: '流通市场流通市场通市场',
        content: '银市场许准予字<br>【2017】第【120】号',
      },
      {
        title: '循环购买',
        content: '否',
      },
    ];
    return (
      <div className="absui-label-value-list">
        <ABSDescription>简单列表：用于简单键值对的说明</ABSDescription>
        <ABSLabelValueList list={list} />
      </div>
    );
  }
}

export default ABSUILabelValueList;