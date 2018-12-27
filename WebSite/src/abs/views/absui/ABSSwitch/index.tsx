import React, { Component } from 'react';
import { ABSSwitch } from '../../../../components/ABSSwitch';
import '../../../../styles/coverAnt.less';
import { Row } from 'antd';
import ABSDescription from '../../../../components/ABSDescription';
import ABSUISimpleContainer from '../../../../components/ABSUISimpleContainer';
import './index.less';

const ABSUISwitchSimpleContainer = ({ children }) => {
  return (
    <ABSUISimpleContainer style={{ width: 80 }}>{children}</ABSUISimpleContainer>
  );
};

class ABSSwitchRoute extends Component {
  switchChange = () => {
    // onChange
  }
  render () {
    return (
      <div className="absui-switch">
        <ABSDescription>开关</ABSDescription>
        <Row style={{ marginBottom: 12}}>
          <ABSUISwitchSimpleContainer><span>关闭</span></ABSUISwitchSimpleContainer>
          <ABSUISwitchSimpleContainer><span>关闭禁用</span></ABSUISwitchSimpleContainer>
          <ABSUISwitchSimpleContainer><span>&nbsp;&nbsp;</span></ABSUISwitchSimpleContainer>
          <ABSUISwitchSimpleContainer><span>开启</span></ABSUISwitchSimpleContainer>
          <ABSUISwitchSimpleContainer><span>开启禁用</span></ABSUISwitchSimpleContainer>
        </Row>
        <Row style={{ marginBottom: 12}}>
          <ABSUISwitchSimpleContainer>
            <ABSSwitch 
              type="on" 
              size="m" 
              disabled={false} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={false} 
            />
          </ABSUISwitchSimpleContainer>
          
          <ABSUISwitchSimpleContainer>
            <ABSSwitch 
              type="off" 
              size="m" 
              disabled={true} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={false} 
            />
          </ABSUISwitchSimpleContainer>

          <ABSUISwitchSimpleContainer><span>&nbsp;&nbsp;</span></ABSUISwitchSimpleContainer>
          
          <ABSUISwitchSimpleContainer>
            <ABSSwitch 
              type="on" 
              size="m" 
              disabled={false} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={true} 
            />
          </ABSUISwitchSimpleContainer>
         
         <ABSUISwitchSimpleContainer>
            <ABSSwitch 
              type="on" 
              size="m" 
              disabled={true} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={true} 
            />
         </ABSUISwitchSimpleContainer>

          {/* <Col span={2}>
            <ABSSwitch 
              type="on" 
              size="m" 
              disabled={true} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={true} 
            />
          </Col> */}
        </Row>

        <Row style={{ marginBottom: 12}}>
          <ABSUISwitchSimpleContainer>
            <ABSSwitch 
              type="on" 
              size="s" 
              disabled={false} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={false} 
            />
          </ABSUISwitchSimpleContainer>
          
          <ABSUISwitchSimpleContainer>
            <ABSSwitch 
              type="off" 
              size="s" 
              disabled={true} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={false} 
            />
          </ABSUISwitchSimpleContainer>

          <ABSUISwitchSimpleContainer><span>&nbsp;&nbsp;</span></ABSUISwitchSimpleContainer>
          
          <ABSUISwitchSimpleContainer>
            <ABSSwitch 
              type="on" 
              size="s" 
              disabled={false} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={true} 
            />
          </ABSUISwitchSimpleContainer>

          <ABSUISwitchSimpleContainer>
            <ABSSwitch 
              type="on" 
              size="s" 
              disabled={true} 
              switchChangeFun={this.switchChange} 
              className=""
              defaultChecked={true} 
            />
          </ABSUISwitchSimpleContainer>
        </Row>
      </div>
      
    );
  }
}

export default ABSSwitchRoute;