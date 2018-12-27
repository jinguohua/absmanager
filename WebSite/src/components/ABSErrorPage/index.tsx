import React from 'react';
import ABSImage from '../ABSImage';
import './index.less';
import routeConfig from '../../abs/config/routeConfig';
import ABSLink from '../ABSLink';

const image = require('../../assets/images/errorimage.png');

export interface IABSErrorPageProps {
  toadress?: string;
}

export interface IABSABSErrorPageState {

}

class ABSErrorPage extends React.PureComponent<IABSErrorPageProps, IABSABSErrorPageState> {
  onRefresh = () => {
    location.reload();
  }

  render() {
    // const {toadress} = this.props;
    return (
      <div className="abs-error-page">
        <ABSImage logo={image} width={205} height={112} />
        <p className="abs-error-page-text">请刷新页面 , 或直接联系我们</p>

        <div className="middle-view">
          <a className="text-style" href="tel:021-31156258">电话 : 请联系021-31156258</a>
          <div className="middle-righr-view">
          <p className="email-text">邮箱 : </p>
          <a className="email-text-style" href="mailto:feedback@cn-abs.com"> feedback@cn-abs.com</a>
          </div>
        </div>

        <div className="foot-view">
          <a onClick={this.onRefresh} >刷新</a>
          <p className="line-view">|</p>
          <ABSLink  to={routeConfig.home} >返回首页</ABSLink>
        </div>
      </div >
    );
  }
}

export default ABSErrorPage;