import React, { ReactNode } from 'react';
import './index.less';
import { ABSButton } from '../ABSButton';
import { ABSModal } from '../ABSModal';

export interface IABSBaseModalProps {
  onConfirm: () => void;
  onClose?: () => void;
  title?: string;
  content: ReactNode;
  width?: number;
  className?: string;
}
 
export interface IABSBaseModalState {
  
}

const DEFAULT_WIDTH = 560;
 
class ABSBaseModal extends React.Component<IABSBaseModalProps, IABSBaseModalState> {
  modal: ABSModal | null;

  onConfirm = () => {
    const { onConfirm } = this.props;
    onConfirm();
  }

  onClose = () => {
    this.show(false);
  }

  show = (show: boolean, callback?: () => void) => {
    if (this.modal) {
      this.modal.setState({ visible: show }, () => {
        if (callback) {
          setTimeout(callback, 100);
        }
      });
    }
  }

  getContent() {
    const { content } = this.props;
    return (
      <div className="base-modal-content">
        {content}
      </div>
    );
  }
  
  render() {
    const { title, width, className } = this.props;
    const content = this.getContent();
    const modalWidth = width ? width : DEFAULT_WIDTH;
    const modalclassName = className ? className : '';
    return (
      <ABSModal
        content={content}
        title={title}
        width={modalWidth}
        className={modalclassName}
        ref={view => this.modal = view}
        destroyOnClose={true}
        footer={[
          <ABSButton
            onClick={this.onConfirm}
            key="submit"
            type="primary"
            icon="check"
          >
            确定
          </ABSButton>,
          <ABSButton
            onClick={this.onClose}
            key="return"
            type="default"
            icon="close"
            style={{marginLeft: '10px'}}
          >
            取消
          </ABSButton>
        ]}
      />
    );
  }
}
 
export default ABSBaseModal;