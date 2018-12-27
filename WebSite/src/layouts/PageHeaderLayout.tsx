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
import commonUtils from '../utils/commonUtils';
import { menuKey as navigationMenuKey } from '../abs/config/navigationMenuKeyConfig';

const { Content, Sider } = Layout;
const CONTENT_MARGIN_TOP = 140;

@authDecorator()
class PageHeaderLayout extends React.Component<any, any> {
  constructor(props: any) {
    super(props);
    this.state = {
      navigationMenus: [],
    };
  }

  componentDidMount() {
    // // 初始化参数（dealId，securityId等）
    // this.props.dispatch({
    //   type: 'global/initParams',
    // });
    // const { menuKey } = this.props; 
    // // const menuKey = 'Investment';
    // const params = commonUtils.getParams();
    // if (menuKey === navigationMenuKey.deal) {
    //   this.props.dispatch({
    //     type: 'global/getScenarios',
    //     payload: {
    //       deal_id: params.deal_id
    //     }
    //   }).then((response) => {
    //     const menus = authUtil.getNavigationMenu(menuKey, response, params.deal_id);
    //     this.setState({ navigationMenus: menus });
    //   });
    // } else {
    //   this.setState({ navigationMenus: authUtil.getNavigationMenu(menuKey) });
    // }
  }

  componentWillReceiveProps(nextprops: any) {
    // 初始化参数（dealId，securityId等）
    // this.props.dispatch({
    //   type: 'global/initParams',
    // });
    const { menuKey } = nextprops; 
    // const menuKey = 'Investment';
    const params = commonUtils.getParams();
    if (menuKey === navigationMenuKey.deal) {
      this.props.dispatch({
        type: 'global/getScenarios',
        payload: {
          deal_id: params.deal_id
        }
      }).then((response) => {
        const menus = authUtil.getNavigationMenu(menuKey, response, params.deal_id);
        this.setState({ navigationMenus: menus });
      });
    } else {
      this.setState({ navigationMenus: authUtil.getNavigationMenu(menuKey) });
    }
  }

  render() {
    const { routerConfig, match, prefix, pageHeader } = this.props;
    const { navigationMenus } = this.state;
    return (
      <>
        {pageHeader}
        <Layout style={{ height: `calc(100vh - ${CONTENT_MARGIN_TOP}px)` }}>
          <Sider style={{ position: 'fixed', left: 0, height: 'inherit' }} width={SIDEBAR_WIDTH}>
            <ABSSidebar menus={navigationMenus} prefix={prefix ? prefix : ''} />
          </Sider>
          <Content style={{ marginLeft: SIDEBAR_WIDTH, height: '100%', overflow: 'hidden' }}>
            <ABSAuthorizedRouteList routerConfig={routerConfig} path={match.path} />
          </Content>
        </Layout>
      </>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global };
}

export default connect(mapStateToProps)(PageHeaderLayout); 
