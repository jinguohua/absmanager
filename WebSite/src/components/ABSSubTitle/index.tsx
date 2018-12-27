import React, { Component } from 'react';
import './index.less';
import classnames from 'classnames';
import { ABSButton } from '../ABSButton';

interface IABSSubTitleProps {
  className?: string;
  title: string;
  renderRight?: any;
  buttonIcon?: string;
  buttonType?: 'primary' | 'default' | 'danger';
  buttonTitle?: string;
  buttonClick?: React.MouseEventHandler<HTMLButtonElement>; // 改为 onClick
}

class ABSSubTitle extends Component<IABSSubTitleProps> {

  renderRight() {
    const { renderRight, buttonIcon, buttonType, buttonTitle, buttonClick } = this.props;
    if (renderRight) {
      return renderRight();
    }
    return (
      <ABSButton icon={buttonIcon} type={buttonType} onClick={buttonClick}>{buttonTitle}</ABSButton>
    );
  }

  render() {
    const { className, title } = this.props;
    const clazs = classnames('abs-sub-title', className);
    return (
      <div className={clazs}>
        <p className="abs-sub-title-left">{title}</p>
        {this.renderRight()}
      </div>
    );
  }
}

export default ABSSubTitle;