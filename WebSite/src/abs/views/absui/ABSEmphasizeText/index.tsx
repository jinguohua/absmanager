import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSEmphasizeText from '../../../../components/ABSEmphasizeText';
import ABSDescription from '../../../../components/ABSDescription';

class ABSUIEmphasizeText extends Component {
  render () {
    return (
      <div className="absui-emphasize-text">
        <ABSDescription>用于金额、笔数数字（强调-大）</ABSDescription>
        <ABSEmphasizeText title="32" style={{ color: '#FF1515'}} />
      </div>
    );
  }
}

export default ABSUIEmphasizeText;