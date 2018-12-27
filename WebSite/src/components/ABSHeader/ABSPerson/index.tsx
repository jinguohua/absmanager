import * as React from 'react';
import { connect } from 'dva';
import { Menu, Avatar } from 'antd';
import Menuhelper from '../ABSMenu/menuhelper';
import { IUser, ISiteMenu } from '../../../utils/authUtil';
import './index.less';
import { ABSAntIcon } from '../../ABSAntIcon';

// const MenuItemGroup = Menu.ItemGroup;

interface IProps {
  menu: ISiteMenu;
  user: IUser;
  firstSelectKey: string;
  thirdSelectKey: string;
  dispatch?: any;
}

interface IState {
  activeKey: string;
}

class ABSPerson extends React.Component<IProps, IState> {
  constructor(props: any) {
    super(props);
    this.state = {
      activeKey: '',
    };
  }

  handleLogout = (e) => {
    this.props.dispatch({
      type: 'global/logout',
    });
  }

  // 重置弹出菜单的定位高度
  menuOpenChange(e: string[]) {
    if (e[0] != null) {
      document.getElementById(e[0] + '$Menu')!.style.marginTop = '-7px';
      this.setState({ activeKey: e[0] });
    } else {
      this.setState({ activeKey: '' });
    }
  }

  render() {
    const SubMenu = Menu.SubMenu;
    const { user, menu, firstSelectKey, thirdSelectKey } = this.props;

    const popoverContent = (
      menu && menu.children ? menu.children.map((element: ISiteMenu) => {
        return (
          <Menu.Item key={element.key}>
            <a href={element.url}>
              <div className="abs-menu-item">
                <div className={element.isNew ? 'abs-reddot' : 'abs-reddot visibility-hidden'} />
                {element.name}
                {!element.isFree ? (element.isOpen ?
                  <ABSAntIcon className="abs-ant-icon-s" type="unlock" /> :
                  <ABSAntIcon className="abs-ant-icon-s" type="lock" />)
                  : null
                }
              </div>
            </a>
          </Menu.Item>
        );
      }) : null
    );

    return (
      <div className="abs-person">
        <div className="abs-menu">
          <Menu selectedKeys={[thirdSelectKey]} mode="horizontal" onOpenChange={(e) => this.menuOpenChange(e)}>
            <SubMenu className="abs-menu-submenu-popup abs-person-popup" key={menu ? menu.key : ''} title={
              <div className="abs-menu-submenu">
                <div className="abs-menu-submenu-title">
                  <div className="abs-person-avatar">
                    <Avatar src={user.avatar} />
                  </div>
                  <div className="abs-person-name">
                    {user.name}
                  </div>
                  <i className="fc fc-direction-down" />
                  <div className={Menuhelper.getArrowClassName(menu ? menu.key : '', firstSelectKey, this.state.activeKey)} />
                </div>
              </div>
            } >
              {popoverContent}
              <Menu.Item className={'abs-menu-item-danger'}>
                <div className="abs-menu-item" onClick={this.handleLogout} >
                  退出登录
                  </div>
              </Menu.Item>
            </SubMenu>
          </Menu>
        </div>
      </div>
    );
  }
}

function mapStateToProps(state: any) {
  return { ...state.account };
}

export default connect(mapStateToProps)(ABSPerson); 
