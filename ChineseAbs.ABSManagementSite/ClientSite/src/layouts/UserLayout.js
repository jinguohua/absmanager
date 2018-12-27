import React, { Fragment } from 'react';
import DocumentTitle from 'react-document-title';
import Link from 'umi/link';
import { Icon } from 'antd';
import GlobalFooter from '@/components/GlobalFooter';
import styles from './UserLayout.less';
import logo from '../assets/logo.svg';

const links = [];

const copyright = (
  <Fragment>
    Copyright <Icon type="copyright" /> 2018 上海联和金融服务有限公司
  </Fragment>
);

class UserLayout extends React.PureComponent {
  // @TODO title
  getPageTitle() {
    this.state = {};
    const title = '系统登陆 - Abs System';
    return title;
  }

  render() {
    const { children } = this.props;
    return (
      <React.Fragment>
        <DocumentTitle title={this.getPageTitle()} />
        <div className={styles.container}>
          <div className={styles.content}>
            <div className={styles.top}>
              <div className={styles.header}>
                <Link to="/">
                  <img alt="logo" className={styles.logo} src={logo} />
                  <span className={styles.title}>ABS System</span>
                </Link>
              </div>
              <div className={styles.desc} />
            </div>
            {children}
          </div>
          <GlobalFooter links={links} copyright={copyright} />
        </div>
      </React.Fragment>
    );
  }
}

export default UserLayout;
