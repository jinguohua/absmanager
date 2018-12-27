import * as React from 'react';
import { connect } from 'dva';
import { Route, Switch } from 'dva/router';
import '../styles/layout.less';
import '../styles/coverAnt.less';
import { Layout } from 'antd';
// import { Layout } from 'antd';
import routerUtil from '../utils/routerUtil';
import 'react-perfect-scrollbar/dist/css/styles.css';
import ABSHomeHeader from '../components/ABSHomeHeader';
import PerfectScrollbar from 'react-perfect-scrollbar';
import extendsRootLayout from '../components/ABSBaseDecorator';
import ABSNotFound from '../components/ABSNotFound';
 
const { Content } = Layout;

@extendsRootLayout()
class UnAuthHeaderContentLayout extends React.Component<any, any> {
  componentDidMount() {
    const { dispatch } = this.props;

    // 获取菜单和版本号
    dispatch({
      type: 'global/getUnauthMenusAndVersion',
    });
  }

  notFound = () => (
    <ABSNotFound />
  )

  render() {
    const { routerConfig, match, menu, isLogin } = this.props;
    const { unauthMenus } = menu;

    return (
      <Layout>
        <ABSHomeHeader menus={unauthMenus} isLogin={isLogin} className="abs-home-header" />
        <Layout style={{ height: '100vh' }} >
          <Content style={{ height: '100%' }}>
            <PerfectScrollbar>
              <Switch>
                {
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
                <Route render={this.notFound} />
              </Switch>
            </PerfectScrollbar>
          </Content>
        </Layout>
      </Layout>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global, ...state.account };
}

export default connect(mapStateToProps)(UnAuthHeaderContentLayout); 
