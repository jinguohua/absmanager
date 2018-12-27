import React from 'react';
import './index.less';
import ABSIcon from '../ABSIcon';
import { ABSAntIcon } from '../ABSAntIcon';
import classNames from 'classnames';

export interface IABSIconTextProps {
  icon: string;
  text: string;
  className?: string;
  isAnt?: boolean;
  style?: React.CSSProperties;
  containerClassName?: string;
  onClick?: (event: React.MouseEvent<any>) => void;
}

export interface IABSIconTextState {

}

class ABSIconText extends React.Component<IABSIconTextProps, IABSIconTextState> {
  render() {
    const { className, isAnt, icon, text, onClick, containerClassName } = this.props;
    const classes = classNames('abs-ant-icon-text', containerClassName);
    return (
      <div className={classes} onClick={onClick}>
        {
          isAnt ?
            <ABSAntIcon className={className} type={icon} />
            :
            <ABSIcon className={className} type={icon} />
        }
        <span className="abs-ant-icon-text-title">{text}</span>
      </div>
    );
  }
}

export default ABSIconText;