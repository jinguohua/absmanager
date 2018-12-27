/**
 * 菜单帮助类
 */
class Menuhelper {
  /**
   * 请求超时设置
   * @param {string} currentKey 当前菜单Key
   * @param {string} selectKey 选中的Key
   * @param {string} activeKey 激活的Key
   * @returns {string} 箭头className
   */
  static getArrowClassName = (currentKey: string, selectKey: string, activeKey: string) => {
    let arrowClassName = 'abs-menu-submenu-arrow ';
    const homeMenuKey = 'HomePage';
    // 一级菜单展开页面三角形
    if (currentKey === selectKey) {
      arrowClassName += 'abs-menu-submenu-arrow-active ';

      if (currentKey === homeMenuKey) {
        arrowClassName += 'abs-menu-submenu-arrow-home ';
      } else {
        arrowClassName += 'abs-menu-submenu-arrow-page ';
      }
    }

    // 一级菜单展开二级菜单三角形
    if (currentKey === activeKey) {
      arrowClassName += 'abs-menu-submenu-arrow-active abs-menu-submenu-arrow-menu ';
    }

    return arrowClassName;
  }
}

export default Menuhelper;
