import React from 'react';
import ABSFooterItem from './ABSFooterItem';
import ABSImage from '../ABSImage';

const company = require('../../assets/images/company-logo.png');
const phone = require('../../assets/images/footer-phone.png');
const home = require('../../assets/images/footer-home.png');
const email = require('../../assets/images/footer-email.png');
const qrcode = require('../../assets/images/qr-code.png');
import './index.less';
import ABSHref from '../ABSHref';

export interface IABSHomeFooterProps {
  
}
 
class ABSHomeFooter extends React.Component<IABSHomeFooterProps> {
  render() { 
    return (
      <div className="abs-home-footer">
        <div className="abs-home-footer-content">
          <div className="abs-home-footer-logo">
            <ABSImage logo={company} style={{ width: 141.5, marginTop: 18.5 }} />
          </div>
          <div className="abs-home-footer-link">
            <ABSFooterItem
              logo={home}
              logoStyle={{ width: 60 }}
              detail={<ABSHref href="http://heyi.io">http://heyi.io</ABSHref>}
            />
            <ABSFooterItem
              logo={phone}
              logoStyle={{ width: 60 }}
              detail={<ABSHref href="tel:021-31156258">021-31156258</ABSHref>}
            />
            <ABSFooterItem
              logo={email}
              logoStyle={{ width: 60 }}
              detail={<ABSHref href="mailto:feedback@cn-abs.com">feedback@cn-abs.com</ABSHref>}
            />
            <ABSFooterItem

              logo={qrcode}
              logoStyle={{ width: 70 }}
              detailStyle={{ marginTop: 5 }}
              detail="微信公众号"
            />
          </div>
        </div>
      </div>
    );
  }
}
 
export default ABSHomeFooter;