/**
 * api config
 */
const API_ADDRESS = process.env.REACT_APP_API_ADDRESS;
const CNABS_API_ADDRESS = API_ADDRESS;

// 全局
export const GlobalApi = {
  unauthMenu: `${CNABS_API_ADDRESS}/global/unauth-menu`, // 无权限菜单
  search: `${CNABS_API_ADDRESS}/global/search/filter`, // 全部搜索
  searchByPage: `${CNABS_API_ADDRESS}/global/search/pager`, // 分页搜
  uploadUrl: `${API_ADDRESS}/common/upload/cnabs`, // 上传文件
  downloadUrl: `${API_ADDRESS}/common/download/cnabs/`, // 下载图片
  uploadImgUrl: `${API_ADDRESS}/common/uploadimg/cnabs`, // 上传图片
  downloadFiles: `${API_ADDRESS}/common/download/cnabs/`, // 下载文件
  filter: `${CNABS_API_ADDRESS}/global/filter`, // 过滤
  dealTypeFilter: `${CNABS_API_ADDRESS}/global/deal-type-filter`, // 详细过滤
  codeitemURL: `${CNABS_API_ADDRESS}/common/dataitems`
};

// 产品
export const ProductApi = {
  fileList: `${CNABS_API_ADDRESS}/deal/document/list`, // 产品文档
  fileListDownload: `${CNABS_API_ADDRESS}/deal/document/download`, // 产品文档下载

  structureImage: `${CNABS_API_ADDRESS}/deal/structure/trade-img`, // 交易结构图片
  triggerEventList: `${CNABS_API_ADDRESS}/deal/structure/trigger-event/list`, // 触发事件
  paymentModel: `${CNABS_API_ADDRESS}/deal/structure/payment-model/chart`, // 偿付模型

  globalDownload: `${CNABS_API_ADDRESS}/global/download`, // 全局下载
  dealList: `${CNABS_API_ADDRESS}/deal/deal-list/pager`, // 产品列表

  dealCore: `${CNABS_API_ADDRESS}/deal/core`, // 关注产品
  dealFollow: `${CNABS_API_ADDRESS}/deal/follow`, // 关注产品
  dealExport: `${CNABS_API_ADDRESS}/deal/export`, // 下载产品

  detail: `${CNABS_API_ADDRESS}/deal/detail`, // 基本要素的详情
  paymentChart: `${CNABS_API_ADDRESS}/deal/security/payment/chart`, // 证券偿付图
  structureChart: `${CNABS_API_ADDRESS}/deal/security/structure/chart`, // 证券结构图
  paymentList: `${CNABS_API_ADDRESS}/deal/analysis/security/payment/list`, // 证券偿付列表（含列表）
  computationResult: `${CNABS_API_ADDRESS}/deal/analysis/security/static-results`, // 证券结果分析、计算结果列表

  assetPoolPaymentTrendChart: `${CNABS_API_ADDRESS}/deal/analysis/asset/payment/chart`, // 资产池偿付走势图
  ICOCTrendChart: `${CNABS_API_ADDRESS}/deal/analysis/asset/icoc/chart`, // IC、OC走势图
  assetPoolInterestRateTrendChart: `${CNABS_API_ADDRESS}/deal/analysis/asset/interest/chart`, // 资产池利率分布走势图

  assetDistribute: `${CNABS_API_ADDRESS}/deal/asset/distribute`, // 资产池分布
  assetRiskList: `${CNABS_API_ADDRESS}/deal/asset/risk/list`, // 资产池风险分析
  assetChangeList: `${CNABS_API_ADDRESS}/deal/asset/change/list`, // 资产池变化情况
  assetDetail: `${CNABS_API_ADDRESS}/deal/asset/detail`, // 原始资产池
  assetPaymentChart: `${CNABS_API_ADDRESS}/deal/analysis/asset/payment/chart`, // 资产池偿付走势图
  assetPrepaymentChart: `${CNABS_API_ADDRESS}/deal/asset/prepayment/chart`, // 资产池累计提前偿还率走势图
  assetDefaultChart: `${CNABS_API_ADDRESS}/deal/asset/default/chart`, // 资产池累计违约率走势图

  securityList: `${CNABS_API_ADDRESS}/deal/security/list`, // 产品要素的证券列表以及产品证券的列表
  originatorDetail: `${CNABS_API_ADDRESS}/deal/originator/detail`, // 主体产品-基本信息
  participateProject: `${CNABS_API_ADDRESS}/deal/originator/deal-list/list`, // 主体产品-参与项目

  toalAsset: `${CNABS_API_ADDRESS}/deal/originator/total-asset/chart`, // 总资产分布图
  liabilityAssetRatio: `${CNABS_API_ADDRESS}/deal/originator/liability-asset-ratio/chart`, // 资产负债率分布图
  assetYield: `${CNABS_API_ADDRESS}/deal/originator/asset-yield/chart`, // 资产收益率分布图
  capitalAbundanceRatio: `${CNABS_API_ADDRESS}/deal/originator/capital-abundance-ratio/chart`, // 资本充足率分布图
  coreCapitalAbundanceRatio: `${CNABS_API_ADDRESS}/deal/originator/core-capital-abundance-ratio/chart`, // 核心资本充足率分布图
  nplRatio: `${CNABS_API_ADDRESS}/deal/originator/npl-ratio/chart`, // 不良贷款率分布图

  scenarioList: `${CNABS_API_ADDRESS}/deal/scenario/list`, // 情景分析列表
  dealSecurityListPager: `${CNABS_API_ADDRESS}/deal/security-list/pager`, // 证券列表
  dealSecurityListExport: `${CNABS_API_ADDRESS}/deal/security-list/export`, // 下载证券列表
  dealListPager: `${CNABS_API_ADDRESS}/deal/deal-list/pager`, // 产品列表
  dealListExport: `${CNABS_API_ADDRESS}/deal/deal-list/export`, // 下载产品列表
  dealPipelinePager: `${CNABS_API_ADDRESS}/deal/pipeline/pager`, // 过会产品列表
  dealPipelineExport: `${CNABS_API_ADDRESS}/deal/pipeline/export`, // 过会产品列表下载
  dealPipelineSchedule: `${CNABS_API_ADDRESS}/deal/pipeline/schedule` // 过会产品流程图
};

// 机构
export const OrganizantionApi = {
  organizationDetail: `${CNABS_API_ADDRESS}/organization/detail`, // 机构信息详情
  rankList: `${CNABS_API_ADDRESS}/organization/rank/list`, // 机构排名列表和图表
  rankExport: `${CNABS_API_ADDRESS}/organization/rank/export` // 机构排名数据下载
};

// 投资
export const InvestmentApi = {
  securityList: `${CNABS_API_ADDRESS}/investment/pager`, // 证券列表
  subscribeSecurity: `${CNABS_API_ADDRESS}/investment/subscribe`, // 证券订阅/取消
  publicTrade: `${CNABS_API_ADDRESS}/investment/publish`, // 发布交易信息
  securityDetail: `${CNABS_API_ADDRESS}/investment/security/detail` // 证券交易详情
};

// 账号
export const AccountApi = {
  login: `${API_ADDRESS}/account/login`, // 登录
  logout: `${CNABS_API_ADDRESS}/account/logout`, // 退出登录
  auth: `${CNABS_API_ADDRESS}/account/auth`, // 获取权限
  accountInfo: `${CNABS_API_ADDRESS}/account/info`, // 个人信息

  resetPassword: `${CNABS_API_ADDRESS}/account/login/password/reset`, // 重置密码
  modifypwd: `${CNABS_API_ADDRESS}/account/password/modify`, // 修改密码

  accountProfileUpdate: `${CNABS_API_ADDRESS}/account/profile/update`, // 编辑个人资料
  profileAvatarUpdate: `${CNABS_API_ADDRESS}/account/profile/avatar/update` // 更新上传头像
};

// 首页
export const HomePageApi = {
  homePage: `${CNABS_API_ADDRESS}/home/hot`, // 登录
  partnerImgs: `${CNABS_API_ADDRESS}/global/partner-imgs` // 合作机构的图片
};

// 定价
export const PriceApi = {
  priceDetailChart: `${CNABS_API_ADDRESS}/pricing/security/price/detail`, // 证券交易定价
  cashflowDetailChart: `${CNABS_API_ADDRESS}/pricing/security/cashflow/detail`, // 证券现金流
  securityCore: `${CNABS_API_ADDRESS}/pricing/security/core`, // 证券核心信息
  securityDetail: `${CNABS_API_ADDRESS}/pricing/security/detail`, // 证券详细信息
  securityPriceDownload: `${CNABS_API_ADDRESS}/pricing/security/price/download`, // 证券交易定价下载

  cashflowDetail: `${CNABS_API_ADDRESS}/pricing/cashflow/detail`, // 现金流总览
  cashflowDownload: `${CNABS_API_ADDRESS}/pricing/cashflow/download`, // 现金流下载
  cashflowDownloadDemo: `${CNABS_API_ADDRESS}/pricing/cashflow/download-demo`, // 现金流下载模板
  cashflowUpload: `${CNABS_API_ADDRESS}/pricing/cashflow/upload`, // 现金流上传
  cashflowDelete: `${CNABS_API_ADDRESS}/pricing/cashflow/delete`, // 现金流删除
  cashflowDifferentChart: `${CNABS_API_ADDRESS}/pricing/cashflow/different`, // 现金流对比
  cashflowRecoveryChart: `${CNABS_API_ADDRESS}/pricing/cashflow/cashflow-recovery`, // 现金流回收分配

  // 交易定价
  tradepriceResult: `${CNABS_API_ADDRESS}/pricing/tradeprice/result`, // 定价结果信息
  tradepriceChart: `${CNABS_API_ADDRESS}/pricing/tradeprice/chart`, // 定价结果与图表信息
  tradepriceCalcdays: `${CNABS_API_ADDRESS}/pricing/tradeprice/calcdays`, // 交易结算日期交互
  tradepriceCalculate: `${CNABS_API_ADDRESS}/pricing/tradeprice/price-calculate`, // 计算
  tradepriceResultDownload: `${CNABS_API_ADDRESS}/pricing/tradeprice/result/download`, // 定价结果下载

  // 收益率
  yieldCurveDetailChart: `${CNABS_API_ADDRESS}/pricing/yieldrate/yieldcurve/detail`, // 收益率曲线图
  spreadModelDetailChart: `${CNABS_API_ADDRESS}/pricing/yieldrate/spreadmodel/detail`, // 利差模型
  barDetailChart: `${CNABS_API_ADDRESS}/pricing/yieldrate/bar/detail`, // 驱动因子利差模型
  lineSeriesDetailChart: `${CNABS_API_ADDRESS}/pricing/yieldrate/lineseries/detail`, // 模型计算实际利差

  customList: `${CNABS_API_ADDRESS}/pricing/scenario/custom/list`, // 获取用户情景列表
  paramList: `${CNABS_API_ADDRESS}/pricing/scenario/custom/param/list`, // 获取产品参考参数
  customSave: `${CNABS_API_ADDRESS}/pricing/scenario/custom/save`, // 新增情景
  customDelete: `${CNABS_API_ADDRESS}/pricing/scenario/custom/delete`, // 删除情景

  // 情景分析
  pricingScenarioList: `${CNABS_API_ADDRESS}/pricing/scenario/list`, // 情景列表和分析结果
  scenarioCalculate: `${CNABS_API_ADDRESS}/pricing/scenario/calculate`, // 情景计算
  cashflowVerify: `${CNABS_API_ADDRESS}/pricing/scenario/cashflow/verify`, // 现金流验证
  cashflowSave: `${CNABS_API_ADDRESS}/pricing/scenario/cashflow/save` // 现金流保存
};

export const ProjectAPI = {
  list: `${CNABS_API_ADDRESS}/project/list`,
  baseInfo: `${CNABS_API_ADDRESS}/project/detail`
};

// 系统管理
export const SystemAPI = {
  userList: `${CNABS_API_ADDRESS}/user/List`, // 用户列表
  createUser: `${CNABS_API_ADDRESS}/user/Create`, // 创建用户
  editUser: `${CNABS_API_ADDRESS}/user/Edit`, // 编辑用户
  deleteUser: `${CNABS_API_ADDRESS}/user/Delete` // 删除用户
};