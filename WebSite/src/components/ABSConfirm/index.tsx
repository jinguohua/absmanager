import React from 'react';
import { Modal } from 'antd';
import './index.less';
import { ABSButton } from '../ABSButton';
import { ABSAntIcon } from '../ABSAntIcon';
import PerfectScrollbar from 'react-perfect-scrollbar';

export interface IABSConfirmProps {
  title: string | React.ReactNode;
  content: string | React.ReactNode;
  type?: 'success'| 'warning' | 'error'; // | 'info' 
  width: number;
  onSuccessCallback: () => boolean|void;
}

export interface IABSConfirmState {
  visible: boolean;
}
export class ABSConfirm extends React.Component<IABSConfirmProps, IABSConfirmState> {
  static defaultProps = {
    type: 'success',
  };

  constructor(props: IABSConfirmProps) {
    super(props);
    this.state = {visible: false};
  }
  handleCancel = () => {
    this.setState({
        visible: false
    });
  }
  handleOK = () => {
    const { onSuccessCallback } = this.props;
    if ( onSuccessCallback && typeof onSuccessCallback === 'function') {
      let result = onSuccessCallback();
      if (!result) {
        this.setState({ visible: false });
      }
    }
  }
  renderFooter() {
    return (
        [
          <ABSButton key="submit" type="primary" className="" onClick={this.handleOK} icon="check">确认</ABSButton>
        ]
    );
  }
  public render() {
    const { title, content, type, width } = this.props;
    const { visible } = this.state;

    // 防止width超界
    const modalWidth = (width > 0 && width <= 1000) ? width : 330;
    
    // 通过弹框类型选择icon类型
    let iconType = 'check-circle';
    switch (type) {
      case 'success':
        iconType = 'check-circle';
        break;
      case 'error':
        iconType = 'close-circle';
        break;
      case 'warning':
        iconType = 'exclamation-circle';
        break;
      // case 'info':
      //   iconType = 'info-circle';
      //   break;
      default:
        break;
    }

    return (
      <div>
        <Modal 
          visible={visible}
          onCancel={this.handleCancel}
          width={modalWidth}
          footer={this.renderFooter()}
          className="abs-confirm-default"
        >
          <h2 className="abs-confirm-title">
            <ABSAntIcon type={iconType} theme="filled" className={`abs-ant-icon-l abs-ant-icon-${type}`} />
          {title}
          </h2>
          <PerfectScrollbar>
            <div className="abs-confirm-content">
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
