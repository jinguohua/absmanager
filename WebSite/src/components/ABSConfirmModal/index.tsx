import React from 'react';
import './index.less';
import ABSBaseModal from '../ABSBaseModal';
import ABSIcon from '../ABSIcon';

export interface IConfirmModalProps {
  onConfirm: () => void;
  title: string;
  icon: string;
}
 
export interface IConfirmModalState {
  
}

class ABSConfirmModal extends React.Component<IConfirmModalProps, IConfirmModalState> {
  modal: ABSBaseModal | null;

  onConfirm = () => {
    const { onConfirm } = this.props;
    onConfirm();
  }

  show = (show: boolean) => {
    if (this.modal) {
      this.modal.show(show);
    }
  }

  render() {
    const { icon, title } = this.props;
    const contents = (
      <div>
        <ABSIcon type={icon} />
        <p>{title}</p>
        <div style={{clear: 'both'}} />
      </div>
    );
    return (
      <div>
        <ABSBaseModal
          content={contents}
          ref={view => this.modal = view}
          onConfirm={this.onConfirm}
          width={360}
          className="abs-expert-confirm-modal"
        />
      </div>
    );
  }
}
 
export default ABSConfirmModal;