using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core
{
    public enum EDealLabProductType
    {
        RMBS = 1,
        CLO = 2,
        AutoLoans = 3,
        Leasing = 4,
        Consume = 5,
        OverrideCF = 6,
        CLOCommon = 7
    }

    public enum ECashflowType
    {
        公共事业收益权 = 1,
        公园景区收益权 = 2,
        航空票款收益权 = 3,
        绿色资源收益权 = 4,
        棚户保障房收益权 = 5,
        热力供应收益权 = 6,
        上网收费收益权 = 7,
        收费路桥收益权 = 8,
        物业租金收益权 = 9,
        学费收益权 = 10
    }

    public enum ECreateMode
    {
        Custom = 1,
        Intelligent = 2
    }
    public enum EDealLabProjectStatus
    {
        Nothing = 0,
        ScheduleSaved = 1,
        ModelSaved = 2,
        Complete = 3,
        Optimizing = 4,
        Optimized = 5,
        OptimizeFailed = 6
    }

    public enum EModelStatus
    {
        Nothing = 0,
        ModelSaved = 1,
        Complete = 2,
        Optimizing = 3,
        Optimized = 4,
        OptimizeFailed = 5
    }

    public enum EDealLabFrequency
    {
        MONTHLY = 12,
        BiMONTHLY = 6,
        QUARTERLY = 4,
        SEMI_ANNUALLY = 2,
        ANNUALLY = 1
    }

    public enum EExchange
    {
        上海证券交易所,
        深证证券交易所,
        银行间市场,
        私募
    }

    public enum ERolling
    {
        向后顺延,
        非跨月向后顺延,
        向前顺延,
        非跨月向前顺延,
        不调整
    }

    public enum EAccrualMethod
    {
        Thirty_360,
        Act_360,
        Act_Act,
        Act_365
    }

    public enum EDeterminationRules
    {
        Simple,
        Complex
    }

    public enum EBasisType
    {
        APB,
        InterestCollection,
        PrincipalCollection,
        OriginalAPB,
        NotesEndingBalance,
        ResidualCashFlow
    }

    public enum EPaymentSequence
    {
        加速清偿触发违约事件,
        违约事件触发加速清偿
    }

    public enum EEodCheckCondition
    {
        最优先级利息未偿付,
        最优先级本金未偿付,
        优先级利息未偿付,
        优先级本金未偿付
    }

    public enum ECLoType
    {
        A类,
        B类,
        C类,
        其它
    }

    public enum EExternalSupport
    {
        无 = 1,
        弱 = 2,
        一般 = 3,
        较强 = 4,
        强 = 5
    }

    public enum ECorporationType
    {
        民企 = 1,
        中外合资 = 2,
        外企 = 3,
        国企 = 4,
        央企 = 5
    }

    public enum EAccountingManagement
    {
        无经审计财务报表 = 1,
        最新财务报告经审计 = 2,
        连续3年或以上经同家机构审计 = 3
    }

    public enum ETechLevel
    {
        落后 = 1,
        较弱 = 2,
        一般 = 3,
        先进 = 4,
        自主研发行业领先 = 5
    }

    public enum EEmployeeLevel
    {
        无学历要求 = 1,
        员工以大专以上学历为主 = 2,
        一般 = 3,
        以本科以上学历为主 = 4,
        领导为业内权威且员工研究生以上学历 = 5
    }

    public enum ERegion
    {
        安徽 = 1,
        北京 = 2,
        福建 = 3,
        甘肃 = 4,
        广东 = 5,
        广西 = 6,
        贵州 = 7,
        海南 = 8,
        河北 = 9,
        河南 = 10,
        黑龙江 = 11,
        湖北 = 12,
        湖南 = 13,
        吉林 = 14,
        江苏 = 15,
        江西 = 16,
        辽宁 = 17,
        内蒙古 = 18,
        宁夏 = 19,
        青海 = 20,
        山东 = 21,
        山西 = 22,
        陕西 = 23,
        上海 = 24,
        四川 = 25,
        天津 = 26,
        西藏 = 27,
        新疆 = 28,
        云南 = 29,
        浙江 = 30,
        重庆 = 31
    }

    public enum EIndustry
    {
        城投 = 1,
        建筑 = 2,
        房地产 = 3,
        钢铁 = 4,
        贵金属 = 5,
        煤炭 = 6,
        能源 = 7,
        机械制造 = 8,
        一般制造 = 9,
        金融服务 = 10,
        汽车 = 11,
        公共服务事业 = 12,
        农业 = 13,
        林业 = 14,
        交通运输 = 15,
        石油化工 = 16,
        商贸 = 17,
        造纸印刷 = 18,
        纺织 = 19,
        化工原料 = 20,
        化工制品 = 21,
        航天军工 = 22,
        电子设备 = 23,
        互联网通信 = 24,
        其他 = 25
    }

    public enum EProductConstitute
    {
        仅生产一种产品且竞争力一般 = 1,
        一到两种具有竞争力的产品 = 2,
        三到四种具有竞争力的产品 = 3,
        有五种以上具有竞争力的产品 = 4
    }

    public enum ESupplierDependence
    {
        主要依赖于单一上游供货商 = 1,
        上游供货渠道较多 = 2,
        拥有自己的上游供货商资源库并按照情况定期调节供货渠道 = 3,
        可实现少量源材料自给 = 4,
        可实现绝大部分上游原料自给 = 5
    }

    public enum ESalesCondition
    {
        仅对单一或少量客户销售 = 1,
        对所在城市客户实现销售覆盖 = 2,
        对所在省份客户实现销售覆盖 = 3,
        对所在大区客户实现销售覆盖 = 4,
        对全国大部分地区客户实现销售覆盖 = 5
    }


    public enum SecurityKey
    {
        ProjectId,
        ProjectGuid,
        CollateralId,
        CollateralGuid,
        ModelId,
        ModelGuid,
        ScenarioId,
        ScenarioGuid
    }


}
