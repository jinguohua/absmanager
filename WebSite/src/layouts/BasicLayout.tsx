import * as React from 'react';
import { connect } from 'dva';
import { Layout } from 'antd';
import PerfectScrollbar from 'react-perfect-scrollbar';
import authDecorator from '../components/ABSAuthDecorator';
import { ABSAuthorizedRouteList } from '../components/ABSAuthorizedRouteList';
import 'react-perfect-scrollbar/dist/css/styles.css';
import '../styles/layout.less';

const { Content } = Layout;

@authDecorator()
class BasicLayout extends React.Component<any, any> {
  componentDidMount() {
    // console.log('BasicLayout did mount');
  }
  render() {
    const { routerConfig, match} = this.props;
    if (routerConfig) {
      return (
        <Content style={{ height: '100%' }}>
          <PerfectScrollbar>
            <ABSAuthorizedRouteList routerConfig={routerConfig} path={match.path} />
          </PerfectScrollbar>
        </Content>
      );
    } else {
      return (
        <div>没有路由</div>
      );
    }
    
  }
}

function mapStateToProps(state: any) {
  return { ...state.global };
}

export default connect(mapStateToProps)(BasicLayout); 
