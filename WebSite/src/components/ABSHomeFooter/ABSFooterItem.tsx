import React from 'react';
import ABSImage from '../ABSImage';
import './ABSFooterItem.less';

export interface IABSFooterItemProps {
  logo: string;
  logoStyle?: React.CSSProperties;
  detail?: string | React.ReactNode;
  detailStyle?: React.CSSProperties;
}
 
class ABSFooterItem extends React.Component<IABSFooterItemProps> {
  render() { 
    const { logo, detail, logoStyle, detailStyle } = this.props;
    return (
      <div className="abs-footer-item">
        <ABSImage logo={logo} style={logoStyle} />
        <p className="abs-footer-item-detail" style={detailStyle}>{detail}</p>
      </div>
    );
  }
}
 
export default ABSFooterItem;