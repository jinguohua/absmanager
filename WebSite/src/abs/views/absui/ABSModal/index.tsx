import React, { Component } from 'react';
import { ABSModal } from '../../../../components/ABSModal';
import { ABSConfirm } from '../../../../components/ABSConfirm';
import { ABSButton } from '../../../../components/ABSButton';
import '../../../../styles/coverAnt.less';
import './index.less';
import { Row } from 'antd';
import ABSDescription from '../../../../components/ABSDescription';

class ABSModalRoute extends Component {
  modal;
  confirm;
  modal2;
  modal3;
  onClickButton = () => {
    this.modal.setState({ visible: true });
  }

  onSuccessCallback = () => {
    // 要不关闭弹窗时 返回true 
    return true;
  }
  openConfirm = () => {
    this.confirm.setState({ visible: true });
  }

  onOpenModal2 = () => {
    this.modal2.setState({ visible: true });
  }

  onOpenModal3 = () => {
    this.modal3.setState({ visible: true });
  }

  onCloseModal3 = () => {
    this.modal3.setState({ visible: false });
  }

  render() {
    const content = (
      <p style={{ margin: 0 }}>请输入您想完善的数据详情，也可以直接拨打电话：021-31156258</p>
    );

    return (
      <div className="absui-modal">
        <ABSDescription>普通弹窗</ABSDescription>
        <Row style={{ marginTop: 12, marginBottom: 12}}>
          <ABSButton
            onClick={this.onClickButton}
            icon="folder-open"
          >
            普通弹窗
          </ABSButton>
        </Row>
        
        <ABSModal
          content={content}
          title="数据完善"
          width={400}
          onSuccessCallback={this.onSuccessCallback}
          ref={view => this.modal = view}
        />

        <Row style={{ marginTop: 12}}>
          <ABSButton
            onClick={this.openConfirm}
            icon="folder-open"
          >
            不带标题弹窗
          </ABSButton>
        </Row>
        
        {/* 隐藏属性： type?: 'success' | 'fail' | 'waring' */}
        <ABSDescription style={{ marginTop: 32}}>不带标题弹窗</ABSDescription>
        <ABSConfirm
          content={content}
          title="数据完善"
          width={360}
          onSuccessCallback={this.onSuccessCallback}
          ref={view => this.confirm = view}
        />

        <Row style={{ marginTop: 12 }}>
          <ABSButton
            onClick={this.onOpenModal2}
            icon="folder-open"
          >
            无标题无底部弹框
          </ABSButton>
        </Row>
        
        <ABSModal
          content={content}
          width={360}
          footer={null}
          ref={view => this.modal2 = view}
        />

        <Row style={{ marginTop: 12 }}>
          <ABSButton
            onClick={this.onOpenModal3}
            icon="folder-open"
          >
            自定义footer弹框
          </ABSButton>
        </Row>
        
        <ABSModal
          title="数据完善"
          content={content}
          width={360}
          footer={ [
            <ABSButton key="submit" type="primary" className="" onClick={this.onCloseModal3} icon="check">继续</ABSButton>,
            <ABSButton key="return" type="danger" className="abs-btn-gap-left" onClick={this.onCloseModal3} icon="close">返回</ABSButton>
          ]}
          ref={view => this.modal3 = view}
        />
      </div >
    );
  }
}

export default ABSModalRoute;