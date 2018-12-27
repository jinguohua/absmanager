import * as React from 'react';
import { connect } from 'dva'; 
import { Layout } from 'antd';
import baseDecorator from '../ABSBaseDecorator';
import ABSHeader from '../../components/ABSHeader';
import ABSErrorBoundary from '../../components/ABSErrorBoundary';
import 'react-perfect-scrollbar/dist/css/styles.css';
import '../../styles/layout.less';
// import commonUtils from '../../utils/commonUtils';

@baseDecorator()
class AuthLayout extends React.Component<any, any> {
  componentDidMount() {
    const { dispatch } = this.props;
    // 初始化参数（dealId，securityId等）
    dispatch({
      type: 'global/initParams',
    });

    // 获取用户权限
    dispatch({
      type: 'global/getAuth',
    });
  }

  componentWillReceiveProps(nextProps: any) {
    // // 初始化参数（dealId，securityId等）
    // const { params } = this.props;
    // const urlParams = commonUtils.getParams();

    // if (params.dealID !== urlParams.deal_id) {
    //   nextProps.dispatch({
    //     type: 'global/initParams',
    //   });
    // }

    // // 初始化参数（dealId，securityId等）
    // nextProps.dispatch({
    //   type: 'global/getAuth',
    // });
  }

  render() {
    const { menu, user, notice, children } = this.props;
    return (
      <Layout>
        <ABSErrorBoundary>
          <ABSHeader menu={menu} user={user} notice={notice} />
          <Layout style={{ height: 'calc(100vh - 60px)' }}>
            {
              children
            }
            {/* <Content style={{ height: '100%' }}>
              <PerfectScrollbar>
                <ABSAuthorizedRouteList routerConfig={routerConfig} path={match.path} />
              </PerfectScrollbar>
            </Content> */}
          </Layout>
        </ABSErrorBoundary>
      </Layout>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.global };
}

export default connect(mapStateToProps)(AuthLayout); 
