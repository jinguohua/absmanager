import React from 'react';
import ABSImage from '../ABSImage';
import './index.less';

const image = require('../../assets/images/no-content.png');

export interface IABSNoContentProps {
  
}
 
export interface IABSNoContentState {
  
}
 
class ABSNoContent extends React.PureComponent<IABSNoContentProps, IABSNoContentState> {
  render() { 
    return ( 
      <div className="abs-no-content">
        <ABSImage logo={image} width={80} height={50}/>
        <p className="abs-no-content-text">暂无数据</p>
      </div>
    );
  }
}
 
export default ABSNoContent;