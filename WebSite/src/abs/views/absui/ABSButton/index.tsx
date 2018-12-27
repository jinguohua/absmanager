import React, { Component } from 'react';
import { ABSButton } from '../../../../components/ABSButton';
import './index.less';
import ABSDescription from '../../../../components/ABSDescription';
import { Row } from 'antd';
import ABSUISimpleContainer from '../../../../components/ABSUISimpleContainer';

const ABSUIButtonSimpleContainer = ({ children }) => {
  return (
    <ABSUISimpleContainer style={{ width: 95 }}>{children}</ABSUISimpleContainer>
  );
};

const ABSUIMiddleButtonSimpleContainer = ({ children }) => {
  return (
    <ABSUISimpleContainer style={{ width: 295 }}>{children}</ABSUISimpleContainer>
  );
};

class ABSButtonRoute extends Component {
  onClick = (e) => {
    alert('按钮被点击');
  }

  render() {
    return (
      <div>
        <ABSDescription>形式1: 图标+文字</ABSDescription>
        <Row style={{ marginBottom: 8 }}>
          <ABSUIButtonSimpleContainer>
            <ABSButton onClick={this.onClick} icon="check" style={{ marginRight: 16}}>订阅</ABSButton>
          </ABSUIButtonSimpleContainer>
         
          <ABSUIButtonSimpleContainer>
            <ABSButton onClick={this.onClick} icon="loading" style={{ marginRight: 16}}>订阅</ABSButton>
          </ABSUIButtonSimpleContainer>
          
          <ABSUIButtonSimpleContainer>
            <ABSButton onClick={this.onClick} icon="check" style={{ marginRight: 16}} type="default">取消</ABSButton>
          </ABSUIButtonSimpleContainer>

          <ABSUIButtonSimpleContainer>
            <ABSButton onClick={this.onClick} icon="check" style={{ marginRight: 16}} type="danger">取消</ABSButton>
          </ABSUIButtonSimpleContainer>
        </Row>

        <Row style={{ marginBottom: 8 }}>
          <ABSUIButtonSimpleContainer>
            <ABSButton onClick={this.onClick} icon="close" disabled={true}>取消</ABSButton>
          </ABSUIButtonSimpleContainer>
         
          <ABSUIButtonSimpleContainer>
            <ABSButton onClick={this.onClick} icon="close" type="default" disabled={true}>取消</ABSButton>
          </ABSUIButtonSimpleContainer>
          
          <ABSUIButtonSimpleContainer>
            <ABSButton onClick={this.onClick} icon="close" type="danger" disabled={true}>取消</ABSButton>
          </ABSUIButtonSimpleContainer>
        </Row>

        <ABSDescription style={{ marginTop: 32 }}>形式2: 图标+文字（高度：40px，总宽度为279px(100%铺满横向)，应用场景如效果图页面，始终固定在底部）</ABSDescription>

        <Row style={{ marginBottom: 12 }}>
          <ABSUIMiddleButtonSimpleContainer><ABSButton onClick={this.onClick} icon="check" style={{ marginRight: 16 }} large={true}>我要申请</ABSButton></ABSUIMiddleButtonSimpleContainer>
          <ABSUIMiddleButtonSimpleContainer><ABSButton onClick={this.onClick} icon="check" style={{ marginRight: 16 }} large={true} type="default">我要申请</ABSButton></ABSUIMiddleButtonSimpleContainer>
          <ABSUIMiddleButtonSimpleContainer><ABSButton onClick={this.onClick} icon="check" style={{ marginRight: 16 }} large={true} type="danger">我要申请</ABSButton></ABSUIMiddleButtonSimpleContainer>
        </Row>

        <ABSDescription style={{ marginTop: 32 }}>形式3: 按钮占据一整行</ABSDescription>
        <Row><ABSButton onClick={this.onClick} icon="check" style={{ marginBottom: 16 }} large={true} block={true}>我要申请</ABSButton></Row>
        <Row><ABSButton onClick={this.onClick} icon="check" style={{ marginBottom: 16 }} large={true} block={true} type="default" >我要申请</ABSButton></Row>
        <Row><ABSButton onClick={this.onClick} icon="close" style={{ marginBottom: 16 }} large={true} block={true} type="danger">特殊按钮</ABSButton></Row>
        
        <Row><ABSButton onClick={this.onClick} icon="check" style={{ marginBottom: 16 }} large={true} block={true} disabled={true}>我要申请</ABSButton></Row>
        <Row><ABSButton onClick={this.onClick} icon="check" style={{ marginBottom: 16 }} large={true} block={true} disabled={true} type="default" >我要申请</ABSButton></Row>
        <Row><ABSButton onClick={this.onClick} icon="close" large={true} block={true} disabled={true} type="danger">特殊按钮</ABSButton></Row>
      </div>
    );
  }
}

export default ABSButtonRoute;
