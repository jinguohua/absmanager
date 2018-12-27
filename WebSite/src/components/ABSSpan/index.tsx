import React from 'react';
import './index.less';

export interface ISpanProps {
  className?: string;
  required?: boolean;
}

export interface ISpanState {

}

class ABSSpan extends React.Component<ISpanProps, ISpanState> {
  renderStar() {
    const { required } = this.props;
    if (!required) { return null; }
    return (
      <span className="base-title-star">*</span>
    );
  }

  render() {
    const { className, children } = this.props;
    return (
      <div className={`base-title ${className}`}>
        {this.renderStar()}
        <span className="base-title-text">{children}</span>
      </div>
    );
  }
}

export default ABSSpan;