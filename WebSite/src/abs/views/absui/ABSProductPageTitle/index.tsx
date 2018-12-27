import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSProductPageTitle from '../../../../components/ABSProductPageTitle';
import { ABSButton } from '../../../../components/ABSButton';
import ABSDescription from '../../../../components/ABSDescription';
import './index.less';

class ABSUIProductPageTitle extends Component {
  render () {
    const buttons = [<ABSButton key="1">按钮1</ABSButton>, <ABSButton key="2">按钮2</ABSButton>];

    return (
      <div className="absui-product-page-title">
        <ABSDescription>产品页面头部</ABSDescription>
        <ABSProductPageTitle fullname="2015年第六期开元信贷资产支持证券" shortname="国家开发银行股份有限公司" buttons={buttons} />
      </div>
    );
  }
}

export default ABSUIProductPageTitle;