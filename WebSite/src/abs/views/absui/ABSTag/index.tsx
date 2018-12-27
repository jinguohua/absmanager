import React, { Component } from 'react';
import { ABSTag } from '../../../../components/ABSTag';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';
import './index.less';

class ABSTagRoute extends Component {
  changehandle = () => {
    // alert
  }
  render () {
    return (
      <div className="absui-tag">
        <ABSDescription>不可点标签</ABSDescription>
          <ABSTag 
            size="default" 
            color="#EE3636" 
            content="互关" 
            visible={true} 
          />
          <ABSTag 
            size="large" 
            color="#EE3636"  
            content="取关" 
            visible={false} 
          />
      </div>
    );
  }
}

export default ABSTagRoute;