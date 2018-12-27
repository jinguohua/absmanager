import React from 'react';
import { Layout, Menu } from 'antd';
import { BrowserRouter, Route, Link, withRouter } from 'react-router-dom';
import styles from './ProDetail.less';
import ProInfo from './ProInfo/ProInfo';
import ProNote from './ProNote/ProNote';

const { Content, Sider } = Layout;

const Home = () => <div>这是Home组件</div>;

const List = props => {
  const { location } = props;

  return (
    <Menu
      defaultSelectedKeys={[location.pathname]}
      selectedKeys={[location.pathname]}
      mode="inline"
    >
      <Menu.Item key="/project/detail/ProInfo">
        <Link to="/project/detail/ProInfo">产品信息</Link>
      </Menu.Item>
      <Menu.Item key="/project/detail/ProNote">
        <Link to="/project/detail/ProNote">支持证券</Link>
      </Menu.Item>
      <Menu.Item key="/project/detail/3">
        <Link to="/project/detail/3">账户信息</Link>
      </Menu.Item>
      <Menu.Item key="/project/detail/4">
        <Link to="/project/detail/4">费用信息</Link>
      </Menu.Item>
      <Menu.Item key="/project/detail/5">
        <Link to="/project/detail/5">资产信息</Link>
      </Menu.Item>
      <Menu.Item key="/project/detail/6">
        <Link to="/project/detail/6">事件与增信</Link>
      </Menu.Item>
    </Menu>
  );
};

const ListWithRouter = withRouter(List);

class SiderDemo extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    return (
      <BrowserRouter>
        <Layout>
          <Sider style={{ backgroundColor: '#f0f2f5' }}>
            <ListWithRouter />
          </Sider>
          <Layout>
            <Content>
              <div className={styles.layout}>
                <Route path="/project/detail/ProInfo" component={ProInfo} />
                <Route path="/project/detail/ProNote" component={ProNote} />
                <Route path="/project/detail/3" component={Home} />
                <Route path="/project/detail/4" component={Home} />
                <Route path="/project/detail/5" component={Home} />
                <Route path="/project/detail/6" component={Home} />
              </div>
            </Content>
          </Layout>
        </Layout>
      </BrowserRouter>
    );
  }
}

export default SiderDemo;
