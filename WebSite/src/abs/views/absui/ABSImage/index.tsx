import React, { Component } from 'react';
import ABSImage from '../../../../components/ABSImage';
const logo = require('../../../../assets/images/qr-code.png');
import './index.less';
import ABSDescription from '../../../../components/ABSDescription';

class ABSImageCard extends Component {
  render () {
    return (
      <div className="abs-image-card">
        <ABSDescription>图片组件</ABSDescription>
        <ABSImage logo={logo} width={80} height={80}/>
      </div>
    );
  }
}

export default ABSImageCard;