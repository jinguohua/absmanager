<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CnabsMenu.ascx.cs" Inherits="ChineseAbs.Menu.CnabsMenu" %>

<link rel="stylesheet" type="text/css" href="/Menu/Styles/content.css?version=20170210" />
<link rel="stylesheet" type="text/css" href="/Menu/Styles/layout.css?version=20161201" />
<link rel="stylesheet" type="text/css" href="/Menu/Styles/cnabs.menu.css?version=20161206S" />
<link rel="stylesheet" type="text/css" href="/Menu/Styles/jquery-ui.css" />
<link rel="stylesheet" type="text/css" href="/Menu/Styles/sidebar.min.css" />
<link rel="stylesheet" type="text/css" href="/Menu/Styles/fancytree/ui.fancytree.css" />
<link rel="stylesheet" type="text/css" href="/Menu/Styles/icon.css" />
<link rel="stylesheet" type="text/css" href="/Menu/Styles/alertify.min.css" />
<link rel="stylesheet" type="text/css" href="/Menu/Styles/alertify.semantic.css" />
<script type="text/javascript" src="/Menu/Scripts/jquery-1.11.1.min.js?version=20161201"></script>
<script type="text/javascript" src="/Menu/Scripts/jquery-ui.min.js"></script>
<script type="text/javascript" src="/Menu/Scripts/cnabsMenu.js?version=20170502" charset="utf-8"></script>
<script type="text/javascript" src="/Menu/Scripts/alertify.js?version=20161201"></script>
<script type="text/javascript" src="/Menu/Scripts/jquery.fancytree.min.js"></script>
<script type="text/javascript" src="/Menu/Scripts/sidebar.js?version=20161201"></script>
<script type="text/javascript" src="/Menu/Scripts/visibility.min.js"></script>

<div id="header">
    <div class="center">
        <a href="https://www.cn-abs.com">
            <div id="divProductName" style="position: relative;">
                <img alt="SAFS" src="/Menu/Images/logo.png" class="logo-img" />
                <%--<img alt="christmas" src="/Images/lantern.png"
                                style="position: absolute; right: -5px; top: 0px;" />--%>
                <div style="padding-top: 12px;">
                    <span class="fixedColor" style="font-size: 18px;">中国资产证券化分析</span>
                    <br />
                    <span class="fixedColor" style="font-size: 12px">China Securitization Analytics</span>
                </div>
            </div>
        </a>
        <div id="divHeaderInfo">
            <div id="divMenuStrip">
                <div class="clearfix">
                    <div class="menu-container">
                        <div class="menu">
                            <ul>
                                <li>
                                    <a href="#"><span>市场分析</span><i class="small caret down icon"></i></a>
                                    <ul>
                                        <div>
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>市场行情</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Market/MarketSummary.aspx") %>" data-lock="市场总览">市场总览</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Market/AssetSummaryV2.aspx")%>" data-lock="细分市场">细分市场</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Market/Ranking.aspx")%>" data-lock="机构份额">机构份额</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Market/cnabsindex.aspx")%>" data-lock="CNABS指数">CNABS指数</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/NegativeWatch.aspx")%>" data-lock="观察名单">观察名单&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>

                                        <div>
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>
                                                            产品数据
                                                        </div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/DealListIssuing.aspx")%>" data-lock="过会信息">过会信息</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/DealListNew.aspx")%>" data-lock="产品列表">产品列表</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/SecurityList.aspx")%>" data-lock="证券列表">证券列表</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/DealFollowList.aspx")%>" data-lock="我的关注">我的关注</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/PrivateDealList.aspx")%>" data-lock="私有产品列表">私有产品列表&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>

                                        <div>
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>
                                                            机构数据
                                                        </div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Organization/OrganizationListV2.aspx")%>" data-lock="机构列表">机构列表&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>

                                        <div style="border: 0;">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>评级研究</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Market/Rating.aspx")%>" data-lock="评级分析">评级分析</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Market/RatingTracing.aspx")%>" data-lock="评级跟踪">评级跟踪&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Market/CNABSGrade.aspx")%>" data-lock="CNABS评分">CNABS评分&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                    </ul>
                                </li>

                                <li>
                                    <a href="#">市场资讯<i class="small caret down icon"></i></a>
                                    <ul>
                                        <div>
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>时事资讯</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/News/List.aspx?listid=1")%>" data-lock="国内新闻">国内新闻</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/News/List.aspx?listid=2")%>" data-lock="国外新闻">国外新闻</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/News/List.aspx?listid=3")%>" data-lock="政策法规">政策法规</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/News/updates.aspx")%>" data-lock="CNABS动态">CNABS动态</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/CNABSDatabase.aspx")%>" data-lock="ABS资料库">ABS资料库&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>

                                        <div style="border: 0">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>活动资讯</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/News/LatestActivity.aspx")%>">会议论坛</a></li>
                                                            <%--<li><a href="javascript:void(0)">奖项评选</a></li>--%>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                    </ul>
                                </li>

                                <li>
                                    <a href="#">产品分析<i class="small caret down icon"></i></a>
                                    <ul style="right: 15%">
                                        <div>
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>
                                                            基础分析
                                                                        <div style="color: #B7AFA5; font-size: 10px">
                                                                            <asp:Label runat="server" ID="lblMasterDealNameData"></asp:Label>
                                                                        </div>
                                                        </div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/DealInformation.aspx")%>" id="menuDealInfo" data-lock="产品信息">产品信息</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/SecuritySummary.aspx")%>" id="menuSecInfo" data-lock="证券信息">证券信息</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/DealModel.aspx")%>" id="menuModel" data-lock="模型结构">模型结构</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Document.aspx")%>" id="menuDocs" data-lock="产品文档">产品文档</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/DealCompare.aspx")%>" data-lock="产品对比">产品对比</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <div style="width: 200px;">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>
                                                            量化分析
                                                                        <div style="color: #B7AFA5; font-size: 10px">
                                                                            <asp:Label runat="server" ID="lblMasterDealNameAnaly"></asp:Label>
                                                                        </div>
                                                        </div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/StaticAnalysis.aspx")%>" id="menuAlysis" data-lock="预设情景分析">预设情景分析</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/ScenarioSetting.aspx")%>" id="menuScenarioSetting" data-lock="自定义情景管理">自定义情景管理&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/CustomScenario.aspx")%>" id="menuCustomScenario" data-lock="自定义任务管理">自定义任务管理&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/CustomStaticAnalysis.aspx")%>" id="menuScenarioResult" data-lock="自定义情景分析">自定义情景分析&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <div style="border: 0;">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>
                                                            定价计算器 
                                                        </div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/PriceYield.aspx")%>" id="menuPrice" data-lock="定价估值">定价估值&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                    </ul>
                                </li>
                                <li>
                                    <a href="#">发行与管理<i class="small caret down icon"></i></a>
                                    <ul style="right: 15%">
                                        <div style="width: 25%;">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div data-lock="产品在线设计">产品在线设计</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_deallabHost,"/MyCollaterals")%>" data-lock="我的资产包">我的资产包&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_deallabHost,"/MyModels")%>" data-lock="我的模型">我的模型&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_deallabHost,"/MyScenarios")%>" data-lock="我的情景">我的情景&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_deallabHost,"/MyProjects")%>" data-lock="我的工程">我的工程&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <div style="width: 25%;">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div data-lock="发行协作平台">发行协作平台</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/ProjectSeries/")%>" data-lock="项目列表">项目列表&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/Dashboard/")%>" data-lock="项目进度">项目进度&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <div style="width: 25%;">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div data-lock="存续期管理平台">存续期管理平台</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/MyProjects/")%>" data-lock="管理产品列表">管理产品列表&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/Schedule/")%>" data-lock="工作列表">工作列表&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/Monitor/")%>" data-lock="产品监控">产品监控&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/Document/")%>" data-lock="文档管理">文档管理&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/BankAccount/")%>" data-lock="产品账户">产品账户&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/PaymentHistory/")%>" data-lock="偿付历史">偿付历史&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/Investment/")%>" data-lock="合格投资">合格投资&nbsp;&nbsp;</a></li>
                                                            <li id="liABSManagerConfig" runat="server"><a href="<%=string.Format("{0}{1}",m_absmanagementHost,"/Configuration/")%>" data-lock="存续期设置">存续期设置&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <div style="width: 25%;">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>产品管理</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/InvestorReport.aspx")%>" data-lock="投资人报告">投资人报告&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <div style="width: 25%; border: 0">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>机构推广</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Organization/Organization.aspx")%>" data-lock="机构页面">机构页面&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/DealSale.aspx")%>" data-lock="产品销售">产品销售&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                    </ul>
                                </li>
                                <li>
                                    <a href="#">投资管理<i class="small caret down icon"></i></a>
                                    <ul style="right: 0">
                                        <div style="width: 50%;">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>组合管理</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/PortfolioManagement/Portfolio.aspx")%>" data-lock="组合编辑器">组合编辑器&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/PortfolioManagement/PortfolioSecurity.aspx")%>" data-lock="组合监控">组合监控&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/PortfolioManagement/PortfolioCalendar.aspx")%>" data-lock="组合日历">组合日历&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <div style="width: 50%; border: 0">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>量化分析</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Analysis.aspx")%>" data-lock="组合分析">组合分析&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Optimizing.aspx")%>" data-lock="组合优化">组合优化&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Valuation.aspx")%>" data-lock="组合估值">组合估值&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/UnderlyingAsset.aspx")%>" data-lock="底层资产分析">底层资产分析&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Message.aspx")%>" data-lock="组合讯息">组合讯息&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                    </ul>
                                </li>
                                <li id="personal" style="float: right; margin-right: 10px" runat="server">
                                    <a href="#" style="position: relative; margin-right: 10px;">
                                        <asp:Label ID="lbLogin" Style="width: 40px; position: relative; left: 17px; top: 3px; font-size: 13px; text-align: center;" runat="server" CssClass="truncate" />
                                        <i class="small caret down icon" style="position: absolute; right: -16px; top: 30px;"></i>
                                        <asp:Image ID="avatar" Style="width: 25px; height: 25px; border-radius: 3px; position: absolute; top: 15px; left: 7px; border: 1px solid rgba(0,0,0,.1); background-color: transparent; box-shadow: 0 1px 0 rgba(255,255,255,.1)" runat="server" />
                                        <span class="message-informer all" style="position: absolute; top: 7px; left: 28px;"></span>
                                    </a>
                                    <ul style="right: 0">
                                        <div>
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>账户信息</div>
                                                        <ul>
                                                            <li id="profile"><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Account/Profile.aspx")%>">个人资料</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Account/Balance.aspx")%>">积分中心</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Account/ChangePassword.aspx")%>">修改密码</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Account/SubManage.aspx")%>">订阅管理</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Deal/PrivateDealApply.aspx")%>">私有产品申请</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost, "/Account/Login.aspx?mode=logout")%>">退出登录</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>

                                        <div>
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>站内公告</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Account/Message.aspx?type=public")%>">站内消息<span class="message-informer cnabs"></span></a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Account/Message.aspx?type=absmanagement")%>">存续期管理消息<span class="message-informer management"></span></a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Account/Message.aspx?type=ticket")%>">工单管理消息<span class="message-informer ticket"></span></a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>

                                        <%-- modify --%>
                                        <div id="adminRole" runat="server">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>CNABS内部管理</div>
                                                        <ul>
                                                            <li id="liAdmin" runat="server"><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Admin/index.aspx")%>">管理功能</a></li>
                                                            <li id="liAnalyst" runat="server"><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Admin/Analyst/AnalystIndex.aspx")%>">Analyst管理功能</a></li>
                                                            <li id="liMarketing" runat="server"><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Admin/Marketing/MarketIndex.aspx")%>">Marketing管理功能</a></li>
                                                            <li id="liAdminNew" runat="server"><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Internal/NewsAudit.aspx")%>">相关新闻审核</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <div id="companyUser" runat="server">
                                            <li>
                                                <ul>
                                                    <li>
                                                        <div>机构账号管理</div>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Enterprise/OrganizationAdmin.aspx?action=content-info")%>" data-lock="机构账户信息">机构账户信息</a></li>
                                                        </ul>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Enterprise/OrganizationAdmin.aspx?action=content-functions")%>" data-lock="功能管理">功能管理</a></li>
                                                        </ul>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Enterprise/OrganizationAdmin.aspx?action=content-users")%>" data-lock="关联用户">关联用户</a></li>
                                                        </ul>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Enterprise/OrganizationAdmin.aspx?action=content-invite")%>" data-lock="用户邀请">用户邀请</a></li>
                                                        </ul>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Enterprise/OrganizationAdmin.aspx?action=content-report")%>" data-lock="机构文件管理">机构文件管理</a></li>
                                                        </ul>
                                                        <ul>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Enterprise/OrganizationAdmin.aspx?action=content-privateDeal")%>" data-lock="机构私有产品">机构私有产品</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>
                                        <%-- 临时工单菜单 --%>
                                        <div style="border: 0;">
                                            <li style="width: 100%">
                                                <ul>
                                                    <li>
                                                        <div>工单管理</div>
                                                        <ul>
                                                            <li id="liReceive" runat="server"><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Tickets/ReceiveTicket.aspx")%>">接受工单</a></li>
                                                            <li id="liSelf" runat="server"><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Tickets/SelfTicketList.aspx")%>">已接工单</a></li>

                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Tickets/Ticket.aspx")%>" data-lock="工单创建">创建工单&nbsp;&nbsp;</a></li>
                                                            <li><a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Tickets/TicketSummary.aspx")%>" data-lock="工单汇总">工单汇总&nbsp;&nbsp;</a></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </div>

                                    </ul>
                                </li>
                                <li id="liLogin" runat="server">
                                    <a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Account/Login.aspx")%>" id="login">登录</a>
                                    <a>|</a>
                                    <a href="<%=string.Format("{0}{1}",m_cnabsHost,"/Apply/Apply.aspx")%>" id="apply">注册</a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<asp:Label ID="hidMasterDealId" runat="server" CssClass="Hide"></asp:Label>
<asp:Label ID="hidLoginStats" runat="server" CssClass="Hide"></asp:Label>
<!--  begin***feedback,user agreement,fast pass icon, sidebar area and custom menu***  -->
<!--feedback template below-->
<div id="dialog" title="请把您的意见和建议告诉我们!" style="text-align: center; padding-top: 6px; display: none">
    <div style="text-align: left; height: 10%">
        问题类型：
              <select id="submit_categary">
                  <option value="function_consult">功能咨询</option>
                  <option value="data_error">数据错误</option>
                  <option value="suggestion">意见建议</option>
                  <option value="other_question">其他问题</option>
              </select>
    </div>
    <div style="width: 100%; height: 90%">
        <textarea id="content" class="feedback-text" wrap="soft" placeholder="您可以输入问题或者建议，我们会及时跟进处理，也可以直接联系咨询官方联系电话：021-31156258"></textarea>
    </div>
</div>
<!--agreement protocol template below-->
<div id="submitTips" style="display: none; text-align: center">正在提交...</div>
<div class="agreement" title="用户许可协议" style="display: none">
    <div>若需继续使用中国资产证券化分析网，您必须接受用户协议：<asp:Label ID="agreementTitle" runat="server"></asp:Label>。</div>
    <asp:Label runat="server" ID="protocol" CssClass="protocol_text"></asp:Label>
    <div style="vertical-align: bottom">
        <input type="checkbox" id="agree" /><label for="agree" style="cursor: pointer">我已阅读并接受《用户协议》中的所有条款</label>
    </div>
</div>
<!--fast pass menu icon below-->
<div id="operationNavigator" style="position: fixed; top: 45%; right: 10px; width: 50px">
    <div id="sub-operation">
        <div class="navIcon" title="产品菜单">
            <i class="big grey ordered list icon iconStyle"></i>
        </div>
        <div class="navIcon" title="签到" id="daily">
            <i class="big icons" style="padding-top: 10px">
                <i class="grey write icon" id="daily-icon"></i>
                <i class="grey corner icon" style="text-shadow: none;">签</i>
            </i>
        </div>
        <div class="navIcon" title="问题反馈">
            <i class="big grey mail icon iconStyle"></i>
        </div>
        <div class="navIcon" title="回到顶部">
            <i class="big grey chevron up icon iconStyle"></i>
        </div>
        <div class="navIcon" title="回到底部">
            <i class="big grey chevron down icon iconStyle"></i>
        </div>
        <div class="navIcon custom-menu" title="自定义菜单">
            <i class="big grey plus icon iconStyle"></i>
        </div>
    </div>
</div>
<!--custom menu icon below-->
<div class="custom-menu-content" title="自定义菜单" style="display: none;">
            <div class="fixedColor" style="margin: 10px 0; padding: 5px; background-color: #5C554D; font-size: 1.2em;">我的菜单</div>
            <div class="my-custom-menu">
            </div>
            <div class="fixedColor" style="margin: 10px 0; padding: 5px; background-color: #5C554D; font-size: 1.2em;">添加更多菜单</div>
            <div class="more-custom-menu">
                <table>
                    <tr>
                        <td class="fixedColor" style="font-size: 1.2em;">市场分析</td>
                    </tr>
                    <tr>
                        <td style="width: 150px;">市场行情</td>
                        <td style="width: 150px;">产品数据</td>
                        <td style="width: 150px;">机构数据</td>
                        <td style="width: 150px;">评级研究</td>
                        <%--                        <td style="width: 150px;">评级研究</td>
                        <td style="width: 150px;">咨询研究</td>--%>
                    </tr>
                    <tr>
                        <td><i class="block layout icon"></i><span>市场总览</span></td>
                        <td><i class="grid layout icon"></i><span>过会信息</span></td>
                        <td><i class="pie chart icon"></i><span>机构列表</span></td>
                        <td><i class="file icon"></i><span>评级分析</span></td>
                        <%-- <td><i class="file text icon"></i><span>国内新闻</span></td>--%>
                    </tr>
                    <tr>
                        <td><i class="grid layout icon"></i><span>细分市场</span></td>
                        <td><i class="list icon"></i><span>产品列表</span></td>
                        <td></td>
                        <td><i class="file video outline icon"></i><span>评级跟踪</span></td>
                        <%--<td><i class="grid layout icon"></i><span>细分市场</span></td>--%>
                        <%--                       
                        <td><i class="book icon"></i><span>国外新闻</span></td>--%>
                    </tr>
                    <tr>
                        <td><i class="pie chart icon"></i><span>机构份额</span></td>
                        <td><i class="unordered list icon"></i><span>证券列表</span></td>
                        <td></td>
                        <td><i class="edit icon"></i><span>CNABS评分</span></td>
                        <%--    <td><i class="pie chart icon"></i><span>机构份额</span></td>
                        
                        <td><i class="file text outline icon"></i><span>政策法规</span></td>--%>
                    </tr>
                    <tr>
                        <td><i class="line chart icon"></i><span>CNABS指数</span></td>
                        <td><i class="file video outline icon"></i><span>我的关注</span>
                            <td></td>
                            <td></td>
                            <%--<td><i class="line chart icon"></i><span>CNABS指数</span></td>
                        <td></td>
                            --%>
                    </tr>
                    <tr>
                        <td><i class="browser icon"></i><span>观察名单</span></td>
                        <td><i class="book icon"></i><span>私有产品列表</span></td>
                        <td></td>
                        <td></td>
                        <%--<td><i class="browser icon"></i><span>观察名单</span></td>
                        <td></td>
                        <td><i class="folder open icon"></i><span>ABS资料库</span></td>--%>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td class="fixedColor" style="font-size: 1.2em;">市场咨询</td>
                    </tr>
                    <tr>
                        <td style="width: 150px;">时事资讯</td>
                        <td style="width: 150px;">活动资讯</td>
                    </tr>
                    <tr>
                        <td><i class="sort alphabet ascending icon"></i><span>国内新闻</span></td>
                        <td><i class="cube icon"></i><span>会议论坛</span></td>
                    </tr>
                    <tr>
                        <td><i class="sort numeric ascending icon"></i><span>国外新闻</span></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td><i class="sitemap icon"></i><span>政策法规</span></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td><i class="file audio outline icon"></i><span>CNABS动态</span></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td><i class="download icon"></i><span>ABS资料库</span></td>
                        <td></td>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td class="fixedColor" style="font-size: 1.2em;">产品分析</td>
                    </tr>
                    <tr>
                        <td style="width: 150px;">基础分析</td>
                        <td style="width: 150px;">量化分析</td>
                    </tr>
                    <tr>
                        <td><i class="sort alphabet ascending icon"></i><span>产品信息</span></td>
                        <td><i class="cubes icon"></i><span>预设情景分析</span></td>
                        <%-- <td><i class="cube icon"></i><span>静态分析</span></td>--%>
                    </tr>
                    <tr>
                        <%--<td><i class="unordered list icon"></i><span>证券列表</span></td>--%>
                        <td><i class="sort numeric ascending icon"></i><span>证券信息</span></td>
                        <td><i class="write icon"></i><span>自定义情景管理</span></td>
                        <%--     <td><i class="cubes icon"></i><span>蒙特卡洛分析</span></td>--%>
                    </tr>
                    <tr>
                        <td><i class="sitemap icon"></i><span>模型结构</span></td>
                        <td><i class="unordered list icon"></i><span>自定义任务管理</span></td>
                    </tr>
                    <tr>
                        <td><i class="download icon"></i><span>产品文档</span></td>
                        <td><i class="browser icon"></i><span>自定义情景分析</span></td>
                    </tr>
                    <tr>
                        <td><i class="pie chart icon"></i><span>产品对比</span></td>
                        <td></td>
                        <%--<td><i class="download icon"></i><span>产品文档</span></td>--%>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td class="fixedColor" style="font-size: 1.2em;">发行与管理</td>
                    </tr>
                    <tr>
                        <td style="width: 150px;">产品在线设计</td>
                        <td style="width: 150px;">发行协作平台</td>
                        <td style="width: 150px;">存续期管理平台</td>
                        <td style="width: 150px;">产品管理</td>
                        <td style="width: 150px;">机构推广</td>
                    </tr>
                    <tr>
                        <td><i class="suitcase icon"></i><span>我的资产包</span></td>
                        <td><i class="options icon"></i><span>项目列表</span></td>
                        <td><i class="indent icon"></i><span>管理产品列表</span></td>
                        <td><i class="file word outline icon"></i><span>投资人报告</span></td>
                        <td><i class="file powerpoint outline icon"></i><span>机构页面</span></td>
                    </tr>
                    <tr>
                        <td><i class="align center icon"></i><span>我的模型</span></td>
                        <td><i class="fast forward icon"></i><span>项目进度</span></td>
                        <td><i class="outdent icon"></i><span>工作列表</span></td>
                        <td></td>
                        <td><i class="dollar icon"></i><span>产品销售</span></td>
                    </tr>
                    <tr>
                        <td><i class="server icon"></i><span>我的情景</span></td>
                        <td></td>
                        <td><i class="search icon"></i><span>产品监控</span></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td><i class="laptop icon"></i><span>我的工程</span></td>
                        <td></td>
                        <td><i class="file pdf outline icon"></i><span>文档管理</span></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td><i class="privacy icon"></i><span>产品账户</span></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td><i class="grid layout icon"></i><span>偿付历史</span></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td><i class="write icon"></i><span>合格投资</span></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td><i class="write icon"></i><span>存续期设置</span></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td class="fixedColor" style="font-size: 1.2em;">投资管理</td>
                    </tr>
                    <tr>
                        <td style="width: 150px;">定价分析</td>
                        <td style="width: 150px;">组合管理</td>
                        <td style="width: 150px;">量化分析</td>
                    </tr>
                    <tr>
                        <td><i class="inbox icon"></i><span>定价估值</span></td>
                        <td><i class="edit icon"></i><span>组合编辑器</span></td>
                        <td><i class="treatment icon"></i><span>组合分析</span></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td><i class="unhide icon"></i><span>组合监控</span></td>
                        <td><i class="h icon"></i><span>组合优化</span></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td><i class="outline calendar icon"></i><span>组合日历</span></td>
                        <td><i class="lab icon"></i><span>组合估值</span></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td><i class="zoom icon"></i><span>底层资产分析</span></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td><i class="newspaper icon"></i><span>组合讯息</span></td>
                    </tr>
                </table>
                <br />
            </div>
            <%--<div class="fixedColor" style="margin:10px 0; padding:5px; background-color:#5C554D; font-size:1.2em;">添加更多功能菜单</div>
            <div class="more-function-menu">
                <table>
                    <tr>
                        <td style="width:150px;"><i class="cherron up icon"></i>回到顶部</td>
                    </tr>
                </table>
            </div>--%>
        </div>

<div class="ui sidebar" style="width: 240px;">
    <div id="fancyTree" class="fancy-tree-style">
    </div>
</div>

<script type="text/javascript">
    $(function () {
        MenuLock();
        MenuLockTip();
    });
    window.loginParam = $("#<%=this.hidLoginStats.ClientID.ToString()%>").html();
    var intervalId = 0;
    var intervalSidebar = 0;
    var fancytreeFlag = 0;
    var dealId = $("#hidMasterDealId").text();

    //fast pass icon function
    $(function () {
        $(".navIcon").click(function (event) {
            var $target = $(this).children("i");
            if ($target.hasClass("list"))
                togl();
            else if ($target.hasClass("mail"))
                feedback();
            else if ($target.hasClass("up"))
                toScrollTop();
            else if ($target.hasClass("down"))
                toScrollBottom();
            else if ($target.hasClass("plus"))
                $('.custom-menu-content').dialog("open")
            else return false;
        });
    });

    //daily check in function
    $(document).ready(function () {
        $.ajax({
            type: "post", 
            url: "/Menu/Ajax/CheckDailyHandler.ashx",
            dataType: "json",
            data: {},
            success: function (data) {
                if (!data.Done) {
                    $("#daily-icon").removeClass().addClass("orange write icon");
                }
            }
        });
    });

    $(function () {
        $("#daily").click(function () {
            if ($("#CnabsMenu_liLogin").length > 0) {
                alertify.alert("请登录后再签到")
            }
            else if ($("#daily-icon").hasClass("grey")) {
                alertify.alert("您今天已经签过到了");
            } else {
                $.ajax({
                    type: "post",
                    url: "/Menu/Ajax/DoDailyHandler.ashx",
                    dataType: "json",
                    data: {},
                    success: function (data) {
                        if (data.OK) {
                            alertify.alert("<p><b>签到成功!</b></p> <p>你已连续签到" + data.Continueation + "天,积分增加" + data.Bonus + ",当前拥有总积分: " + data.Balance + ".</p>");
                            $("#daily-icon").removeClass().addClass("grey write icon");
                        } else {
                            alertify.alert("<p><b>签到失败!</b></p> <p>" + data.Err + ".</p>");
                        }
                    }
                })
            }
        });
    });

    $(document).ready(function () {
        var $sidebar = $(".ui.sidebar");
        var $body = $("body");
        window.setInterval(getLatestDealNews, 300 * 1000);
        intervalId = window.setInterval(getUnreadMsgCount, 300 * 1000);
        intervalSidebar = window.setInterval(function(){ 
            !$sidebar.hasClass("visible") && ($body.removeClass("pushable") && $sidebar.hide())
            if ($sidebar.hasClass("uncover"))
                $sidebar.removeClass("animating");
        }, 3* 1000);
        getUnreadMsgCount();
        FeedbackTemplate();
        CustomMenuInitial();
        SubmitTips();
        NavigatorFocus();
           
        if (<%=this.isAccept%> == 0)
        {
            $(".agreement").dialog({
                closeText: "",
                autoOpen: true,
                closeOnEscape: false,
                show: true,
                hide: true,
                draggable: false,
                resizable: false,
                height: 450,
                width: 700,
                modal: true,
                open:function(event,ui){
                    $(".ui-dialog-titlebar-close").hide();
                },
                buttons: {
                    "同意": function () {
                        var $button = $(".ui-dialog-buttonpane.ui-widget-content.ui-helper-clearfix button:first");
                        $button.attr("disabled","disabled");
                        var IsAccept = $("#agree").is(":checked");
                        if (IsAccept == false)
                            alert("请阅读并接受用户协议");
                        if (IsAccept == true) {
                            $.ajax({
                                url: "/Menu/Ajax/WebAgreementHandler.ashx",
                                error: function(XMLHttpRequest, textStatus, errorThrown) {
                                    alert(textStatus);
                                },
                            })
                            $(this).dialog("close");
                        }
                    },
                    "拒绝": function () {
                        window.location.href = "/Account/Login.aspx?mode=logout";
                    }
                },
            });
        }
    });
</script>
