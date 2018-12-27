/**
 * 页面跳转路由配置
 */
export default class RouteConfig {
  /**
   * 根目录
   */
  static root: string | undefined = process.env.REACT_APP_PUBLISH_PATH;

  /**
   * 宣传页
   */
  static introdution: string = RouteConfig.root + '';

  /**
   * 首页
   */
  static home: string = RouteConfig.root + 'index.html#/main/home';

  /**
   * 搜索结果
   */
  static search: string = RouteConfig.root + 'index.html#/main/search';

  /**
   * 登陆
   */
  static login: string = RouteConfig.root + 'account.html#/login';

  /**
   * 注册
   */
  static register: string = RouteConfig.root + 'account.html#/main/register';

  /**
   * 注册成功
   */
  static registerSuccess: string = RouteConfig.root + 'account.html#/success/register';

  /** 
   * 产品基本分析
   */
  static dealStaticAnalysis: string = RouteConfig.root + 'product.html#/detail/basic-analysis';

  /**
   * 证券信息
   */
  static investmentSecurityInfo: string = RouteConfig.root + 'investment.html#/security/info?security_id=';

  /** 
   * 产品要素
   */
  static productDealInfo: string = RouteConfig.root + 'product.html#/detail/basic-info';

  /** 
   * 证券定价-定价
   */
  static investmentSecurityPricing: string = RouteConfig.root + 'investment.html#/security/pricing';

  /** 
   * 证券定价-现金流
   */
  static investmentSecurityCashflow: string = RouteConfig.root + 'investment.html#/security/cashflow';

  /** 
   * 机构详情
   */
  static organizationDetail: string = RouteConfig.root + 'organization.html#/detail/info?organization_id=';

  /** 
   * 产品列表
   */
  static productList: string = RouteConfig.root + 'product.html#/list/product';

  static chartCnabsLink: string = 'http://cn-abs.com';
}