import * as React from 'react';
import { connect } from 'dva';
import { Layout } from 'antd';
import authDecorator from '../components/ABSAuthDecorator';
import { ABSAuthorizedRouteList } from '../components/ABSAuthorizedRouteList';
import 'react-perfect-scrollbar/dist/css/styles.css';
import '../styles/layout.less';
import ABSSidebar from '../components/ABSSidebar';
import { SIDEBAR_WIDTH } from '../utils/constant';
import authUtil from '../utils/authUtil';
import ABSContainer from '../components/ABSContainer';

const { Content, Sider } = Layout;

@authDecorator()
class SideBarLayout extends React.Component<any, any> {
  render() {
    const { routerConfig, match, menu, menuKey, removePadding } = this.props;
    const navigationMenus = authUtil.getSubMenu(menu.navigationMenus, menuKey);

    return (
      <>
        <Sider style={{ position: 'fixed', left: 0, height: 'inherit' }} width={SIDEBAR_WIDTH}>
          <ABSSidebar menus={navigationMenus} />
        </Sider>
        <Content style={{ marginLeft: SIDEBAR_WIDTH, height: '100%' }}>
          <ABSContainer removePadding={removePadding}>
            <ABSAuthorizedRouteList routerConfig={routerConfig} path={match.path} />
          </ABSContainer>
        </Content>
      </>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global };
}

export default connect(mapStateToProps)(SideBarLayout); 
