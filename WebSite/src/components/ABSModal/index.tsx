import React from 'react';
import { Modal } from 'antd';
import './index.less';
import { ABSButton } from '../ABSButton';
import PerfectScrollbar from 'react-perfect-scrollbar';

export interface IABSModalProps {
  className?: string;
  content: React.ReactNode;
  title?: string;
  width: number;
  footer?: React.ReactNode | null;
  // 成功之后的回调
  onSuccessCallback?: () => boolean | void;
  removePaddingX?: boolean;
  destroyOnClose?: boolean;
}

export interface IABSModalStates {
  visible: boolean;
}

export class ABSModal extends React.Component<IABSModalProps, IABSModalStates> {
  constructor(props: IABSModalProps) {
    super(props);
    this.state = {
      visible: false,
    };
  }

  handleOK = () => {
    const { onSuccessCallback } = this.props;
    if (onSuccessCallback && typeof onSuccessCallback === 'function') {
      let result = onSuccessCallback();
      if (!result) {
        this.setState({ visible: false });
      }
    }
  }

  handleCancel = () => {
    this.setState({ visible: false });
  }

  renderFooter() {
    return (
      this.props.footer ? this.props.footer :
        this.props.footer == null && this.props.footer !== undefined
          ? null :
          [
            <ABSButton key="submit" type="primary" className="" onClick={this.handleOK} icon="check">确认</ABSButton>,
            <ABSButton key="return" type="default" className="abs-btn-gap-left" onClick={this.handleCancel} icon="close">取消</ABSButton>
          ]

    );
  }

  render() {
    const {
      content,
      title,
      width,
      className,
      footer,
      removePaddingX,
      destroyOnClose,
    } = this.props;
    const { visible } = this.state;
    // 防止width超界
    const modalWidth = (width > 0 && width <= 1000) ? width : 330;
    const noFooter = (footer === null) ? 'abs-modal-body-nofooter-scrollbar' : '';
    const isRemovePaddingX = removePaddingX ? 'nopaddingx' : '';
    return (
      <div className="abs-modal">
        <Modal
          title={title}
          visible={visible}
          width={modalWidth}
          onCancel={this.handleCancel}
          footer={this.renderFooter()}
          className={className + ' abs-modal-default '}
          destroyOnClose={destroyOnClose}
        >
          <PerfectScrollbar>
            <div className={`abs-modal-body-scrollbar ${noFooter} ${isRemovePaddingX}`}>
              <div>
                {content}
              </div>
            </div>
          </PerfectScrollbar>
        </Modal>
      </div>
    );
  }
}
