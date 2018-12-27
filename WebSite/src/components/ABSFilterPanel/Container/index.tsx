import React from 'react';
import './index.less';
import classNames from 'classnames';
import ABSContainer from '../../ABSContainer';
import { ABSButton } from '../../ABSButton';

export interface IContainerProps {
  onConfirm: (event: any) => void;
  onClose?: (flag: boolean) => void;
}

export interface IContainerState {
  hide: boolean;
}

class Container extends React.PureComponent<IContainerProps, IContainerState> {
  state = {
    hide: false,
  };

  onClose = (event: any) => {
    const { onClose } = this.props;
    if (onClose) { onClose(false); }
  }

  switchPanel = () => {
    const { onClose } = this.props;
    if (onClose) { onClose(!this.state.hide); }
    this.setState((prevState) => ({ hide: !prevState.hide }));
  }

  render() {
    const { onConfirm, children } = this.props;
    const { hide } = this.state;
    const panelLeftStyle = classNames('abs-filter-panel-left', { 'abs-filter-panel-left-hide': hide });
    const switchBtnStyle = classNames('abs-filter-panel-right-button', { 'abs-filter-panel-right-button-hide': hide });
    return (
      <div className="abs-filter-panel">
        <div className={panelLeftStyle}>
          <div className="abs-filter-panel-tip">
            <div className="abs-filter-panel-tip-dot" />
            <span>筛选条件</span>
          </div>
          <ABSContainer className="abs-filter-panel-content" style={{ padding: 0 }}>
            <div className="abs-filter-panel-content-spacing">
              {children}
            </div>
          </ABSContainer>
          <div className="abs-filter-panel-bottom">
            <ABSButton
              onClick={onConfirm}
              icon="check"
              style={{ marginRight: 10 }}
            >
              确认
            </ABSButton>
            <ABSButton
              onClick={this.onClose}
              icon="close"
              type="default"
            >
              取消
            </ABSButton>
          </div>
        </div>
        <div className="abs-filter-panel-right">
          <div className={switchBtnStyle} onClick={this.switchPanel} />
        </div>
      </div>
    );
  }
}

export default Container;