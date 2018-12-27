import React from 'react';
import classNames from 'classnames';

export interface IABSHrefProps {
  href: string;
  children: any;
  className?: string;
  style?: React.CSSProperties;
}
 
class ABSHref extends React.Component<IABSHrefProps> {
  render() { 
    const { href, children, className } = this.props;
    const classes = classNames('abs-href', className);
    return (
      <a className={classes} href={href}>{children}</a>
    );
  }
}
 
export default ABSHref;