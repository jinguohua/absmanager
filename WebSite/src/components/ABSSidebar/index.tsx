import * as React from 'react';
import { Menu } from 'antd';
import PerfectScrollbar from 'react-perfect-scrollbar';
import ABSIcon from '../ABSIcon';
import './index.less';
import ABSLink from '../ABSLink';
import commonUtils from '../../utils/commonUtils';
import classNames from 'classnames';

const SubMenu = Menu.SubMenu;

export interface IABSSidebarMenuData {
  icon: string;
  name: string;
}

interface IABSSidebarProps {
  location?: any;
  menus?: any;
  prefix?: string;
}

class ABSSidebar extends React.Component<IABSSidebarProps, any> {
  static defaultProps = {
    prefix: '',
  };

  renderIcon(item: IABSSidebarMenuData) {
    return (
      <span>
        <span><ABSIcon type={item.icon} /><span>{item.name}</span></span>
      </span>
    );
  }

  renderMenu = (menus) => {
    if (menus && menus.length > 0) {
      return menus.map((item, index) => {
        const key = item.url ? item.url : item.id;

        if (item.children && item.children.length > 0) {
          return (
            <SubMenu title={this.renderIcon(item)} key={key}>
              {this.renderMenu(item.children)}
            </SubMenu>
          );
        }
        const smallSubMenuClassName = classNames({
          'small-submenu-title': item.url.includes('basic-analysis'),
        });
        return (
          <Menu.Item key={key}>
            <ABSLink to={`${item.url}`} className={smallSubMenuClassName}>
              <ABSIcon type={item.icon} />
              <span>{item.name}</span>
            </ABSLink>
          </Menu.Item>
        );
      });
    }
    
    return null;
  }

  render() {
    const {  menus } = this.props;
    const selectedKey = commonUtils.getSelectedKey();
    return (
      <PerfectScrollbar className="abs-sidebar-container">
        <Menu
          defaultOpenKeys={['1']}
          mode="inline"
          theme="dark"
          selectedKeys={[selectedKey]}
          inlineIndent={20}
        >
          {this.renderMenu(menus)}
        </Menu>
      </PerfectScrollbar>
    );
  }
}

export default ABSSidebar;