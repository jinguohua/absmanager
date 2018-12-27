using ABS.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Helpers
{
    public class MyEnumConvertor
    {
        public readonly static Dictionary<string, string> MyDictionary = new Dictionary<string, string>()
        {
            {"",""},
            //PAYMENTFREQUENCY
            {"DAILY","日付"},
            {"WEEKLY","周付"},
            {"SEMI_MONTHLY","半月付"},
            {"MONTHLY","月付"},
            {"BiMONTHLY","两月付"},
            {"QUARTERLY","季付"},
            {"SEMI_ANNUALLY","半年付"},
            {"ANNUALLY","年付"},
            //DEALLABPROJECTTYPE
            {"RMBS","住房抵押贷款"},
            {"CLO","企业贷款"},
            {"AutoLoans","汽车抵押贷款"},
            {"Leasing","融资租赁"},
            {"Consume","个人消费贷款" },
            {"CLOCommon","通用型企业债权" },
            {"OverrideCF","现金流" },
            //ACCRUALMETHOD
            {"Thirty_360","30/360"},
            {"Act_360","Act/360"},
            {"Act_Act","Act/Act"},
            {"Act_365","Act/365"},
            //DETERMINATIONRULE
            {"Simple","简单"},
            {"Complex","组合"},
            //BASISTYPE
            {"APB","当前资产池金额"},
            {"InterestCollection","利息收入帐金额"},
            {"PrincipalCollection","本金收入帐金额"},
            {"OriginalAPB","原始资产池金额"},
            {"NotesEndingBalance","所有证券剩余本金之和" },
            {"ResidualCashFlow","剩余现金流" },
            //FLOATINGINDEX
            {"CNS003M","3个月存款基准利率"},
            {"CNS006M","6个月存款基准利率"},
            {"CNS012M","1年存款基准利率"},
            {"CNS003Y","3年存款基准利率"},
            {"CNS005Y","5年存款基准利率"},
            {"CNS010Y","10年存款基准利率"},
            {"CNL006M","6个月贷款基准利率"},
            {"CNL012M","1年贷款基准利率"},
            {"CNL003Y","3年贷款基准利率"},
            {"CNL005Y","5年贷款基准利率"},
            {"CNL010Y","10年贷款基准利率"},
            {"CNHL005Y","5年住房公积金基准利率"},
            {"CNHL010Y","10年住房公积金基准利率"},
            //ANALYSIS
            {"NPV","净现值"},
            {"Loss","损失"},
            {"IRR","内部收益率"},
            //PROJECTSTATUS
            {"Nothing","新建"},
            {"ScheduleSaved","已保存设置"},
            {"Complete","已完成"},
            {"Optimizing","自动测算中"},
            {"Optimized","自动测算完成"},
            {"OptimizeFailed","自动测算失败"},
            //ASSETMODEL
            {"资产编号","ID"},
            {"封包日","AsOfDate"},
            {"资产种类","AssetType"},
            {"借款人","Borrower"},
            {"贷款名","LenderName"},
            {"计日方式","RememberDayType"},
            {"入池本金","InitialCorpus"},
            {"是否是浮动利率","HasFloatRate"},
            {"基准利率","BaseRate"},
            {"票面利率或息差","CouponSpread"},
            {"入池折扣利率","CouponDiscount"},
            {"本金还款方式","PrincipalRepaymentType"},
            {"账龄(年)","LoanAge"},
            {"还款周期","Frequency"},
            {"到期日","DueDate"},
            {"剩余期限(年)","WAL"},
            {"行业","Industry"},
            {"评级","AssetRating"},
            {"担保方式","AssureMode"},
            {"担保方","Sponsor"},
            {"租赁方式","RentType"},
            {"承租方","Tenant"},
            {"租赁合同编号","ContractNumber"},
            {"租赁年利率或利差","RentRateByYear"},
            {"保证金比例","MarginRatio"},
            {"租金计算方式","RentCalcMode"},
            {"还款日期","RepaymentDay"},
            {"还款金额","RepaymentSum"},
            {"备注","Comment" },
            {"贷款价值比","Ltv"},
            {"借款人收入债务比","DebtRatio"},
            {"借款人行业","Industry"},
            {"地区","Region"},
            {"入池利率折扣率","CouponDiscount"},
            {"年利率或手续费率/息差","CouponSpread"},
            //ASSETRATINGEXCHANGE
            {"C","A"},
            {"CC","B"},
            {"CCC-","C"},
            {"CCC","D"},
            {"CCC+","E"},
            {"B-","F"},
            {"B","G"},
            {"B+","H"},
            {"BB-","I"},
            {"BB","J"},
            {"BB+","K"},
            {"BBB-","L"},
            {"BBB","M"},
            {"BBB+","N"},
            {"A-","O"},
            {"A","P"},
            {"A+","Q"},
            {"AA-","R"},
            {"AA","S"},
            {"AA+","T"},
            {"AAA-","U"},
            {"AAA","V"},
            { "最优先级利息未偿付", "最优先级利息未偿付"},
            { "最优先级本金未偿付", "最优先级本金未偿付"},
            { "优先级利息未偿付", "优先级利息未偿付"},
            { "优先级本金未偿付", "优先级本金未偿付"},
            // 模型 增信 触发顺序
            {"加速清偿触发违约事件","加速清偿触发违约事件" },
            { "违约事件触发加速清偿","违约事件触发加速清偿"}
        };

        public readonly static Dictionary<string, string> MyRemindSettingDictionary = new Dictionary<string, string>()
        {
            {"",""},

            { "H12", "12" },
            { "H24", "15" },
            { "H48", "39" },
            { "短信", "1" },
            { "邮件", "2" },
            { "短信加邮件", "3" },

            { "1", "短信" },
            { "2", "邮件" },
            { "3", "短信加邮件" },
            { "12", "H12" },
            { "15", "H24" },
            { "39", "H48" },

            // EFrequency
            {"DAILY","日付"},
            {"WEEKLY","周付"},
            {"SEMI_MONTHLY","半月付"},
            {"MONTHLY","月付"},
            {"BiMONTHLY","两月付"},
            {"QUARTERLY","季付"},
            {"SEMI_ANNUALLY","半年付"},
            {"ANNUALLY","年付"},

            // EDeterminationRules
            {"Simple","简单"},
            {"Complex","组合"},

            {"向后顺延","向后顺延"},
            {"非跨月向后顺延","非跨月向后顺延"},
            {"向前顺延","向前顺延"},
            {"非跨月向前顺延","非跨月向前顺延"},
            {"不调整","不调整"},

            //BASISTYPE
            {"APB","当前资产池金额"},
            {"InterestCollection","利息收入帐金额"},
            {"PrincipalCollection","本金收入帐金额"},
            {"OriginalAPB","原始资产池金额"},
            {"NotesEndingBalance","所有证券剩余本金之和" },
            {"ResidualCashFlow","剩余现金流" },
            
            // 模型 增信 触发顺序
            {"加速清偿触发违约事件","加速清偿触发违约事件" },
            { "违约事件触发加速清偿","违约事件触发加速清偿"},
            //EEodCheckCondition
            { "最优先级利息未偿付", "最优先级利息未偿付"},
            { "最优先级本金未偿付", "最优先级本金未偿付"},
            { "优先级利息未偿付", "优先级利息未偿付"},
            { "优先级本金未偿付", "优先级本金未偿付"},

             //ACCRUALMETHOD
            {"Thirty_360","30/360"},
            {"Act_360","Act/360"},
            {"Act_Act","Act/Act"},
            {"Act_365","Act/365"},

        };

        public readonly static Dictionary<string, string> MyContactDictionary = new Dictionary<string, string>()
        {
            {"",""},

            { "原始权益人", "1" },
            { "底层资产", "2" },
            { "托管行", "3" },
            { "登记托管机构", "4" },
            { "交易场所", "5" },
            { "计划管理人", "6" },
            { "投资人", "7" },
            { "评级机构", "8"},
            { "担保人", "9"},
            { "律师事务所", "10"},
            { "会计师事务所", "11"},
            { "财务顾问", "12"},
            { "差额支付承诺人", "13"},
            { "承销商", "14"},
            { "发起机构", "15"},
            { "发行人", "16"},
            { "监管银行", "17"},
            { "评估机构", "18"},
            { "受托机构", "19"},
            { "税务顾问", "20"},
            { "委托机构", "21"},
            { "项目安排人", "22"},
            { "证券化服务商", "23"},
            { "资产服务机构", "24"},
            { "资金保管机构", "25"},
            { "其它", "26"},

            { "1", "原始权益人" },
            { "2", "底层资产" },
            { "3", "托管行" },
            { "4", "登记托管机构" },
            { "5", "交易场所" },
            { "6", "计划管理人" },
            { "7", "投资人" },
            { "8", "评级机构" },
            { "9", "担保人" },
            { "10", "律师事务所" },
            { "11", "会计师事务所" },
            { "12", "财务顾问"},
            { "13", "差额支付承诺人"},
            { "14", "承销商"},
            { "15", "发起机构"},
            { "16", "发行人"},
            { "17", "监管银行"},
            { "18", "评估机构"},
            { "19", "受托机构"},
            { "20", "税务顾问"},
            { "21", "委托机构"},
            { "22", "项目安排人"},
            { "23", "证券化服务商"},
            { "24", "资产服务机构"},
            { "25", "资金保管机构"},
            { "26", "其它"},
        };

        public readonly static Dictionary<string, string> AssetPoolDic = new Dictionary<string, string>() {
            {"合同编号","ContractNo"},
            {"资产种类","SecurityType"},
            {"入池本金","PrincipalBalance"},
            {"是否是浮动利率","IsFloating"},
            {"基准利率","BaseRate"},
            {"还款周期","PaymentFrequency"},
            {"到期日","DueDate"},
            {"剩余期限(年)","Wal"},
            {"账龄(年)","LoanAge"},
            {"地区","Region"},
            {"备注","Comment"}
        };

        public static string GetValueByKey(Object obj)
        {
            if (obj == null)
                return "";
            string key = obj.ToString();
            return MyDictionary.ContainsKey(key) ? MyDictionary[key] : key;
        }

        public static List<SelectListItem> ConvertToSelectList(Type enumerationType, string select = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "-请选择-" });
            var values = Enum.GetValues(enumerationType);
            Dictionary<string, string> dic = null;
            if (enumerationType.Name == "EAccountType")
                dic = MyDictionary;
            else if (enumerationType.Name == "EDutyType")
                dic = MyContactDictionary;
            else
                dic = MyRemindSettingDictionary;

            foreach (var value in values)
            {
                Enum a = (Enum)value;
                list.Add(new SelectListItem()
                {
                    Text = a.ToString(),
                    Value = dic[a.ToString()],
                    Selected = a.ToString().Equals(select)
                });
            }
            return list;
        }


    }



}