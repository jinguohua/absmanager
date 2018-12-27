import * as React from 'react';
import ABSMenu from './ABSMenu';
import ABSSearch from './ABSSearch';
import ABSPerson from './ABSPerson';
import { IUser, ISiteMenu, INavigationMenu } from '../../utils/authUtil';
import routeConfig from '../../abs/config/routeConfig';
import './index.less';
import ABSLink from '../ABSLink';

const logo = require('../../assets/images/logo.png');
const homepageKey = 'HomePage';
const personCenterKey = 'PersonCenter';

interface IProps {
  user: IUser;
  menu: {
    siteMenus: Array<ISiteMenu>;
    navigationMenus: Array<INavigationMenu>;
    personMenu: ISiteMenu;
  };
  notice: any;
}

interface IState {
  firstSelectKey: string;
  thirdSelectKey: string;
  isShowSearch: boolean;
}

let flatMenus: Array<ISiteMenu> = [];
let flatNavigationMenus: Array<INavigationMenu> = [];

class ABSHeader extends React.Component<IProps, IState> {
  constructor(props: any) {
    super(props);
    this.state = {
      firstSelectKey: '',
      thirdSelectKey: '',
      isShowSearch: true
    };
  }

  componentWillReceiveProps(nextprops: IProps) {
    const { menu } = nextprops;
    const { siteMenus, navigationMenus, personMenu } = menu;
    let tempSiteMenus = siteMenus.map(item => {
      return item;
    });

    tempSiteMenus.push(personMenu);

    this.getFlatMenuData(tempSiteMenus);
    this.getFlatNavigationMenuData(navigationMenus);
    this.getSelectMenuKey();
    this.checkIsShowSearch();
  }

  // 获取平级菜单
  getFlatMenuData(menus: any) {
    menus.map(item => {
      item.url = item.url ? item.url.toLowerCase() : null;
      flatMenus.push(item);

      if (item.children) {
        this.getFlatMenuData(item.children);
      }
    });
  }

  // 获取侧边平级菜单
  getFlatNavigationMenuData(menus: any) {
    menus.map(item => {
      item.url = item.url ? item.url.toLowerCase() : null;
      flatNavigationMenus.push(item);

      if (item.children) {
        this.getFlatNavigationMenuData(item.children);
      }
    });
  }

  // 获取当前选中的菜单Key
  getSelectMenuKey() {
    let firstSelectKey: string = '';
    let thirdSelectKey: string = '';
    const { pathname, search, hash } = window.location;
    let fullPathname = pathname + search + hash;
    fullPathname = fullPathname.toLowerCase();
    // 首页匹配
    if (pathname === '/') {
      thirdSelectKey = homepageKey;
    } else {
      // 根据当前Url匹配
      for (let i = 0; i < flatMenus.length; i++) {
        const item = flatMenus[i];
        if (
          item.url &&
          fullPathname.indexOf(item.url) > -1 &&
          item.children.length === 0
        ) {
          thirdSelectKey = item.key;
          break;
        }
      }

      // 如果匹配不到，从导航规则中匹配
      if (thirdSelectKey === '') {
        for (let j = 0; j < flatNavigationMenus.length; j++) {
          const item = flatNavigationMenus[j];
          if (item.url && fullPathname.indexOf(item.url) > -1) {
            thirdSelectKey = item.siteMenuKey ? item.siteMenuKey : '';
            break;
          }
        }
      }
    }

    const thirdMenu = flatMenus.find(r => r.key === thirdSelectKey);

    if (thirdMenu) {
      const secordMenu = flatMenus.find(r => r.id === thirdMenu.parentId);

      if (secordMenu) {
        if (secordMenu.key === personCenterKey) {
          firstSelectKey = personCenterKey;
        } else {
          const firstMenu = flatMenus.find(r => r.id === secordMenu.parentId);
          firstSelectKey = firstMenu ? firstMenu.key : homepageKey;
        }
      }
    }

    this.setState({
      firstSelectKey,
      thirdSelectKey
    });
  }

  checkIsShowSearch = () => {
    const { hash } = window.location;
    if (hash && (hash.indexOf('search') >= 0 || hash.indexOf('home') >= 0)) {
      this.setState({
        isShowSearch: false
      });
    }
  }

  render() {
    const { menu, user } = this.props;
    const { siteMenus, personMenu } = menu;

    const { firstSelectKey, thirdSelectKey, isShowSearch } = this.state;
    const loginContent = () => {
      if (user.isLogin) {
        return (
          <div>
            <ABSPerson
              menu={personMenu}
              user={user}
              firstSelectKey={firstSelectKey}
              thirdSelectKey={thirdSelectKey}
            />
            {/* <ABSNotice data={messageCount} /> */}
            {isShowSearch ? <ABSSearch /> : null}
          </div>
        );
      } else {
        return null;
      }
    };

    return (
      <div className="abs-header">
        <div className="abs-header-logo">
          <ABSLink to={routeConfig.home}>
            <img src={logo} />
          </ABSLink>
        </div>
        <ABSMenu
          menu={siteMenus}
          firstSelectKey={firstSelectKey}
          thirdSelectKey={thirdSelectKey}
        />
        <div className="abs-header-right">{loginContent()}</div>
      </div>
    );
  }
}

export default ABSHeader;
