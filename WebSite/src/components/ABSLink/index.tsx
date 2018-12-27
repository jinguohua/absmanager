import React from 'react';
import classNames from 'classnames';
import { Link } from 'react-router-dom';

export interface IABSHrefProps {
  to: string;
  children: any;
  className?: string;
  target?: '_blank' | '_self';
  title?: string;
  // 是否阻止事件冒泡
  stopPropagation?: boolean;
}

export default class ABSLink extends React.Component<IABSHrefProps> {

  onClick = (e) => {
    // e.preventDefault();
    // 阻止冒泡
    const { stopPropagation } = this.props;
    if (stopPropagation) {
      e.stopPropagation();
    }
  }

  render() {
    const { to, children, className, target, title } = this.props;
    const classes = classNames('abs-href', className);
    let newTarget;
    if (target) {
      newTarget = target;
    } else if (to.indexOf('http') > -1) {
      newTarget = '_blank';
    } else {
      newTarget = '_self';
    }

    return (
      to.indexOf('html') < 0 && to.indexOf('http') < 0 ?
        <Link to={to} className={classes} title={title}>{children}</Link>
        :
        <a className={classes} href={to} target={newTarget} title={title} onClick={this.onClick}>{children}</a>
    );
  }
}
