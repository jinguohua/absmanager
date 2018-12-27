import React from 'react';
import { Anchor } from 'antd';
import classNames from 'classnames';

const { Link } = Anchor;

export interface IABSAnchorPointLink {
  href: string;
  title: string;
}
export interface IABSAnchorPointProps {
  affix?: boolean;
  links: Array<IABSAnchorPointLink>;
  className?: string;
  style?: React.CSSProperties;
}
 
export interface IABSAnchorPointState {
  
}
 
class ABSAnchorPoint extends React.Component<IABSAnchorPointProps, IABSAnchorPointState> {

  renderLinks() {
    const { links } = this.props;
    if (links) {
      return links.map((link) => <Link href={link.href} title={link.title} key={`${link.href}${link.title}`} />);
    }
    return null;
  }

  render() { 
    const { className } = this.props;
    const classes = classNames('abs-anchor-point', className);
    return (
      <Anchor affix={false} className={classes}>
        {this.renderLinks()}
      </Anchor>
    );
  }
}
 
export default ABSAnchorPoint;