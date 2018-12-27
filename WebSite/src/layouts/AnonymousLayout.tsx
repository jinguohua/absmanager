import * as React from 'react';
import { connect } from 'dva';
import { Route } from 'dva/router';
import '../styles/layout.less';
import '../styles/coverAnt.less';
import { Layout } from 'antd';
import routerUtil from '../utils/routerUtil';
import 'react-perfect-scrollbar/dist/css/styles.css';
import ABSRegisterHeader from '../components/ABSRegisterHeader';
import PerfectScrollbar from 'react-perfect-scrollbar';
// import ABSFooter from '../components/ABSFooter';
import extendsRootLayout from '../components/ABSBaseDecorator';
 
const { Content } = Layout;

@extendsRootLayout()
class RegisterHeaderContentLayout extends React.Component<any, any> {
  render() {
    const { routerConfig, match, menu, isLogin  } = this.props;
    const {unauthMenus} = menu;

    return (
      <Layout>
        <ABSRegisterHeader menus={unauthMenus} isLogin={isLogin} className="abs-register-header" />
        <Layout  style={{ height: '100vh'}} >
          <Content className={'abs-register-layout'} style={{ height: '100%' }}>
            <PerfectScrollbar>
              {
                // 自动生成路由
                routerUtil.getRoutes(match.path, routerConfig).map(item => {
                  return (
                    <Route
                      key={item.key}
                      path={item.path}
                      component={item.component}
                    />
                  );
                })
              }
            </PerfectScrollbar>
          </Content>
        </Layout>
        {/* <ABSFooter /> */}
      </Layout>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global, ...state.account };
}

export default connect(mapStateToProps)(RegisterHeaderContentLayout); 
