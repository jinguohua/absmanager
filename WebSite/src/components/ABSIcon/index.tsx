import React from 'react';
import classNames from 'classnames';
import '../../assets/font-cop/css/font-cop.css';

export interface IABSIconProps {
  type: string;
  style?: React.CSSProperties;
  className?: string;
  onClick?: () => void;
}

class ABSIcon extends React.Component<IABSIconProps> {
  render() {
    const {
      className,
      type,
      onClick,
    } = this.props;
    const classes = classNames('abs-icon', 'fc', className, {
      [`fc-${type}`]: type,
    });
    return <i className={classes} onClick={onClick} />;
  }
}

export default ABSIcon;