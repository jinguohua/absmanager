using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.LogicModels.DealModel;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.Models.DealModel;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Utils.DatasetTable;
using ChineseAbs.CalcService.Data.NancyData;
using ChineseAbs.Logic;
using ChineseAbs.Logic.Object;
using CNABS.Mgr.Deal.Model;
using CNABS.Mgr.Deal.Model.Result;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CNABS.Mgr.Deal.Utils
{
    public static class ABSDealUtils
    {
        public static void GenerateNextModel(ABSDeal absDeal)
        {

        }

        public static DataTable GetCashflowDt(DatasetScheduleLogicModel datasetSchedule,
            AssetCashflowVariable assetCashflowVariable,
            double sysPredictInterest, double sysPredictPrincipal,
            out double currInterestCollection, out double currPrincipalCollection)
        {
            currInterestCollection = sysPredictInterest;
            currPrincipalCollection = sysPredictPrincipal;

            var assetOverrideSetting = new AssetOverrideSetting(assetCashflowVariable);
            DataTable cashflowDt = new DataTable();
            var project = datasetSchedule.ProjectLogicModel.Instance;
            var dataset = datasetSchedule.Dataset.Instance;
            var paymentDate = datasetSchedule.PaymentDate;

            var hasOsa = false;
            var osa = datasetSchedule.Dataset.DealModel.OverrideSingleAsset;
            foreach (var asset in datasetSchedule.Dataset.Assets)
            {
                var overridePrincipal = osa.GetPrincipal(asset.AssetId);
                var overrideInterest = osa.GetInterest(asset.AssetId);
                if (overridePrincipal != null || overrideInterest != null)
                {
                    hasOsa = true;
                    break;
                }
            }

            //测算优先级：
            //1、覆盖现金流测算
            //2、单笔资产覆盖的方式测算
            //3、系统测算
            //未填写覆盖现金流值，或填写值和系统测算值一致，认为是不使用本息覆盖现金流测算
            var hasOverrideCashflow = assetCashflowVariable != null
                && assetCashflowVariable.EnableOverride;

            if (hasOverrideCashflow)
            {
                //使用覆盖现金流的方式测算
                cashflowDt = NancyUtils.GetStaticAnalyticsResult(project.ProjectId, null, dataset.AsOfDate, assetOverrideSetting).CashflowDt;
                currInterestCollection = assetOverrideSetting.Interest;
                currPrincipalCollection = assetOverrideSetting.Principal;
            }
            else
            {
                if (hasOsa)
                {
                    //未填写覆盖现金流参数，已填写单笔资产覆盖
                    //重新计算本息，使用覆盖现金流的方式测算
                    var dealModel = datasetSchedule.Dataset.DealModel;
                    var absDeal = new ABSDeal(dealModel.YmlFolder, dealModel.DsFolder);

                    var acfResult = absDeal.Result.AcfResult;
                    var acfDataset = acfResult.Dataset.SingleOrDefault(x => x.PaymentDay == paymentDate);
                    acfResult.MergeOsa(osa);
                    acfResult.ReCalcSum();

                    var sumPrincipal = double.Parse(acfDataset.Sum.Principal.ToString("n2"));
                    var sumInterest = double.Parse(acfDataset.Sum.Interest.ToString("n2"));

                    assetOverrideSetting.IsOverride = true;
                    assetOverrideSetting.Principal = sumPrincipal;
                    assetOverrideSetting.Interest = sumInterest;
                    assetOverrideSetting.PaymentDate = paymentDate;

                    cashflowDt = NancyUtils.GetStaticAnalyticsResult(project.ProjectId, null, dataset.AsOfDate, assetOverrideSetting).CashflowDt;
                    currInterestCollection = assetOverrideSetting.Interest;
                    currPrincipalCollection = assetOverrideSetting.Principal;
                }
                else
                {
                    //未填写覆盖现金流参数，且未填写单笔资产覆盖
                    //使用非覆盖现金流的方式测算
                    cashflowDt = NancyUtils.GetStaticAnalyticsResult(project.ProjectId, null, dataset.AsOfDate).CashflowDt;
                }
            }

            return cashflowDt;
        }

        public static ABSDealResult GetNancyDealResult(ABSDeal absDeal)
        {
            var nancyResult = GetNancyStaticAnalysisResult(absDeal);
            return GetNancyDealResult(absDeal, nancyResult);
        }

        public static ABSDealResult GetNancyDealResult(ABSDeal absDeal, NancyStaticAnalysisResult nancyResult)
        {
            var result = new ABSDealResult();
            result.OriginAcf = nancyResult.AssetCashflowDt;
            result.OriginCf = nancyResult.CashflowDt;
            result.Acf = result.OriginAcf.Copy();
            result.Cf = result.OriginCf.Copy();
            result.AcfResult = new AcfResult(result.OriginAcf, absDeal.Assets);

            RunSpecialRules(result.Cf, absDeal.Info.IsCsdcDeal);

            SplitCategoryByNote(result.Cf);

            result.Cf = CleanAndTranslateCashflowTable(result.Cf);
            result.Acf = CleanAndTranslateAssetCashflowTable(result.Acf);

            AddAssetIdToRepeatedCNName(result.Acf, absDeal);

            var dictDates = GetDeterminationDatesByPaymentDates(absDeal);
            ConvertAcfColumnDateName(result.Acf, dictDates);

            return result;
        }

        public static DataTable CleanAndTranslateAssetCashflowTable(DataTable cf)
        {
            DataTable rt = new DataTable();
            rt = cf.AsEnumerable().Where(x =>
                !m_assetIgnoredCashflowType.Contains(x[1].ToString())
                ).CopyToDataTable();

            for (int i = rt.Rows.Count - 1; i >= 0; i--)
            {
                var row = rt.Rows[i];

                if (row[1].ToString() != "" && row[0].ToString() == "" && row[1].ToString().Contains("Total"))
                {
                    row[0] = "总计";
                }

                if (m_dicAssetCashflowType.ContainsKey(row[1].ToString()))
                {
                    row[1] = m_dicAssetCashflowType[row[1].ToString()];
                }

                foreach (var key in m_dicAssetCashflowType.Keys)
                {
                    if (row[1].ToString().IndexOf(key) > -1)
                    {
                        row[1] = row[1].ToString().Replace(key, m_dicAssetCashflowType[key]);
                    }
                }
            }
            foreach (DataColumn c in rt.Columns)
            {
                if (DateTime.TryParse(c.ColumnName, out DateTime date))
                {
                    c.ColumnName = date.ToString("yyyy-MM-dd");
                }
                else if (c.ColumnName.Equals("Security"))
                {
                    c.ColumnName = "资产";
                }
                else if (c.ColumnName.Equals("Type"))
                {
                    c.ColumnName = "项目";
                }
            }

            for (int i = rt.Rows.Count - 1; i >= 0; i += -1)
            {
                DataRow row = rt.Rows[i];
                if ((row[0] == null || row[0].ToString() == "") && (row[1] == null || row[1].ToString() == ""))
                {
                    rt.Rows.Remove(row);
                }
                else if (string.IsNullOrEmpty(row[0].ToString()) && string.IsNullOrEmpty(row[1].ToString()))
                {
                    rt.Rows.Remove(row);
                }
            }
            return rt;
        }

        public static DataTable CleanAndTranslateCashflowTable(DataTable cf)
        {
            DataTable rt = new DataTable();
            rt = cf.AsEnumerable().Where(x =>
                (!m_ignoredCategories.Contains(x[0].ToString()) && !m_ignoredSubCategories.Contains(x[1].ToString()))
                || (x[0].ToString() == "Variables" && x[1].ToString() == "中证手续费应付")
                ).CopyToDataTable();

            var variableIndex = rt.IndexOfRow(x => x[0].ToString() == "Variables" && x[1].ToString() == "中证手续费应付");
            var accountIndex = rt.IndexOfRow(x => x[0].ToString() == "Reserve Accounts");
            if (variableIndex != -1 && accountIndex != -1)
            {
                var rowVariable = rt.Rows[variableIndex];
                DataRow newRow = rt.NewRow();
                // We "clone" the row
                newRow.ItemArray = rowVariable.ItemArray;
                // We remove the old and insert the new
                rt.Rows.Remove(rowVariable);
                rt.Rows.InsertAt(newRow, accountIndex);
            }

            foreach (DataRow row in rt.Rows)
            {
                foreach (var key in m_dictCategories.Keys)
                {
                    if (row[0].ToString().IndexOf(key) > -1)
                    {
                        row[0] = row[0].ToString().Replace(key, m_dictCategories[key]);
                    }
                }
                foreach (var key in m_dictSubCategories.Keys)
                {
                    if (row[1].ToString().IndexOf(key) > -1)
                    {
                        row[1] = row[1].ToString().Replace(key, m_dictSubCategories[key]);
                    }
                }
            }

            for (int i = rt.Columns.Count - 1; i >= 0; i--)
            {
                var c = rt.Columns[i];
                if (c.ColumnName.Equals("Total"))
                {
                    rt.Columns.Remove(c);
                }
            }

            rt.RemoveAllRow(x => x[1].ToString() == "TotalExpense余额"
                || x[1].ToString() == "OtherIncome余额");

            foreach (DataColumn c in rt.Columns)
            {
                if (DateTime.TryParse(c.ColumnName, out DateTime date))
                {
                    c.ColumnName = date.ToString("yyyy-MM-dd");
                }
                else if (c.ColumnName.Equals("Category"))
                {
                    c.ColumnName = "类型";
                }
                else if (c.ColumnName.Equals("Description"))
                {
                    c.ColumnName = "项目描述";
                }
            }

            for (int i = rt.Rows.Count - 1; i >= 0; i += -1)
            {
                DataRow row = rt.Rows[i];
                if ((row[0] == null || row[0].ToString() == "")&&(row[1] == null || row[1].ToString() == ""))
                {
                    rt.Rows.Remove(row);
                }
                else if (string.IsNullOrEmpty(row[0].ToString())&& string.IsNullOrEmpty(row[1].ToString()))
                {
                    rt.Rows.Remove(row);
                }
            }
            return rt;
        }

        private static string[] m_ignoredCategories = new string[] {
            "Floating Indexes",
            "Test Detail",
            "Variables",
            "Macros"
        };

        private static string[] m_ignoredSubCategories = new string[] {
            "TransferToPrincipal.End Balance",
            "Margin_1",
            "Margin_2",
            "Margin_3",
            "Guarantee_1",
            "Guarantee_2",
            "Guarantee_3",
            "InvestmentAccount.End Balance"
        };

        private static Dictionary<string, string> m_dictCategories = new Dictionary<string, string>()
        {
            {"Floating Indexes","参考利率"},
            {"Collateral","资产池"},
            {"Fees","费用"},
            {"Reserve Accounts","账户"},
            {"Accounts","账户"},
            {"Notes","支持证券"},
            {"Tests","事件结果"},
            {"Test Detail","事件计算"},
            {"Variables","自定义变量"},
            {"Macros","宏变量"}
        };

        private static Dictionary<string, string> m_dictSubCategories = new Dictionary<string, string>()
        {
            {"APB","剩余本金"},
            {"Loss.Loss","实际损失"},
            {"Loss","损失"},
            {"InterestCollection","利息收入账户"},
            {"PrincipalCollection.Unscheduled","非计划本金收入"},
            {"PrincipalCollection","本金收入账户"},
            {"MarginAccount","保证金"},
            {"Margin","差额支付事件"},
            {"Guarantee","担保事件"},
            {".Amount Due","预计支付"},
            {".Received","的实际支付"},
            {".Beginning Deferred","的逾期金额"},
            {".Deferred Received","的逾期金额支付"},
            {".Deferred Interest Received","收到逾期利息"},
            {".Interest Deferred","的未偿利息"},
            {".Interest On Deferred Interest Received","收到逾期复利"},
            {".End Balance","余额"},
            {".Beginning Principal Outstanding","的期初本金"},
            {".Principal Received","的本金支付"},
            {".Beginning Interest Due","的预计利息金额"},
            {".Interest Received","收到的利息"},
            {".Equity Distribution","的次级收入"},
            {"CsdcFee", "中证手续费" },
            {"EOD_TEST","违约事件测试"},
            {"EOD","违约事件"},
            {"Turbo","加速清偿事件"},
            {"TransferToPrincipal","转入本金"},
            {"Reinvestment", "循环购买"},
            {"ExpenseAccount", "费用账户"},
            //{"InvestmentAccount","合格投资收益" },
            {"ReserveAmount", "留存账户"},
            {"Sub","次级" },
            {"Trustee","托管" },
            {"Service","服务" },
            {"Fee","费" }
        };

        private static Dictionary<string, string> m_dicAssetCashflowType = new Dictionary<string, string>()
        {
            {"Loss","损失"},
            {"Interest","利息"},
            {"Principal","本金"},
            {"Performing","剩余本金"},
            {"Defaulted","违约"},
            {"Received","合计"},
            {"Total ","总"},
            {"Total Received","合计"},
            {"Fee","费用" }
        };


        private static string[] m_assetIgnoredCashflowType = new string[] 
        {
            "Total Loss PV",
            "Cumulative Defaulted"
        };


        private static Dictionary<string, string> m_dicCurrentVariables = new Dictionary<string, string>()
        {
            {".Loss.Amount","损失"},
            {".InterestCollection","利息回收款"},
            {".PrincipalCollection","本金回收款"},
            {"Collateral","资产池"},
            {"InvestmentAccount", "当期合格投资收益" }
        };

        private static Dictionary<string, string> m_dicFutureVariables = new Dictionary<string, string>()
        {
            {"Fee","费用"},
            {"Expense","其他费用"},
            {"Audit","审计"},
            {"Rating","跟踪评级"},
            {"Trustee","托管" },
            {"OtherIncomeMax","其它收益" },
            {"Reserve","留存" },
        };

        private static Dictionary<string, string> m_dicCurrentVariablesCn2En = new Dictionary<string, string>()
        {
            {"损失",".Loss.Amount"},
            {"利息回收款",".InterestCollection"},
            {"本金回收款",".PrincipalCollection"},
            {"资产池","Collateral"},
            {"当期合格投资收益","InvestmentAccount"}
        };

        private static Dictionary<string, string> m_dicFutureVariablesCn2En = new Dictionary<string, string>()
        {
            {"其他费用","Expense"},
            {"费用","Fee"},
            {"审计","Audit"},
            {"跟踪评级","Rating"},
            {"托管","Trustee" },
            {"留存","Reserve" },
        };

        /// <summary>
        /// 根据当期PaymentDate获取PaymentDates对应的DeterminationDates
        /// </summary>
        public static Dictionary<DateTime, DateTime> GetDeterminationDatesByPaymentDates(ABSDeal absDeal)
        {
            var dealSchedule = absDeal.Schedule;
            var paymentDates = dealSchedule.PaymentDates;
            var determinationDates = dealSchedule.DeterminationDates;

            var paymentDateStrList = paymentDates.ToList().ConvertAll(x => x.ToString());
            var dictionaryDates = new Dictionary<DateTime, DateTime>();

            for (int i = 0; i < paymentDates.Length; i++)
            {
                dictionaryDates[paymentDates[i]] = determinationDates[i];
            }

            return dictionaryDates;
        }

        private static void ConvertAcfColumnDateName(DataTable acf, Dictionary<DateTime, DateTime> dictDates)
        {
            for (int i = 0; i < acf.Columns.Count; ++i)
            {
                var c = acf.Columns[i];
                if (DateTime.TryParse(c.ColumnName, out DateTime date))
                {
                    if (dictDates.Keys.Contains(date))
                    {
                        acf.Columns[i].ColumnName = DateUtils.DateToString(dictDates[date]);
                    }
                }
            }
        }

        private static void AddAssetIdToRepeatedCNName(DataTable acf, ABSDeal absDeal)
        {
            var assets = absDeal.Assets;
            var rowSpansDic = SetRowSpansDic(acf);
            List<string> acfTableCNNames = new List<string>();

            for (int iRow = 0; iRow < acf.Rows.Count; iRow++)
            {
                var row = acf.Rows[iRow];
                var cnName = row[0].ToString();
                if (rowSpansDic[iRow] != 0 && cnName != string.Empty && cnName != "总计")
                {
                    acfTableCNNames.Add(cnName);
                }
            }

            var assetsCNNames = assets.Select(x => x.SecurityData.SecurityName).ToList();
            var diffAcfTableCNNames = acfTableCNNames.Distinct().ToList();
            var diffAssetsCNNames = assetsCNNames.Distinct().ToList();
            CommUtils.Assert(diffAcfTableCNNames.Count == diffAssetsCNNames.Count, "资产表格行数与资产数量不相等");

            object lastName = string.Empty;
            int iAssetsRow = 0;
            for (int iRow = 0; iRow < acf.Rows.Count; iRow++)
            {
                var acfTableRow = acf.Rows[iRow];
                CommUtils.Assert(acfTableRow.ItemArray.Length > 0, "");
                if (acf.Rows[iRow][0].ToString() == string.Empty
                    || acf.Rows[iRow][0].ToString() == "总计")
                {
                    continue;
                }
                if (rowSpansDic[iRow] != 0)
                {
                    iAssetsRow++;
                    lastName = acfTableRow[0].ToString();
                }

                CommUtils.Assert(iAssetsRow >= 1 && iAssetsRow < assets.Count + 1, "资产assets下标[{0}]越界({1}-{2})", iAssetsRow, 1, assets.Count + 1);
                var assetId = assets[iAssetsRow - 1].SecurityData.AssetId;
                var acfTableCNName = acfTableCNNames[iAssetsRow - 1];
                var assetsCNName = assetsCNNames[iAssetsRow - 1];

                var acfTableCNNameCount = acfTableCNNames.FindAll(x => x == acfTableCNName).ToList().Count();
                var assetsCNNameCount = assetsCNNames.FindAll(x => x == assetsCNName).ToList().Count();
                CommUtils.Assert(acfTableCNNameCount == assetsCNNameCount, "资产[{0}]数量有误", acfTableCNName);

                if (acfTableCNNameCount > 1)
                {
                    if (rowSpansDic[iRow] != 0)
                    {
                        lastName = acfTableCNName + "(" + assetId.ToString() + ")";
                        assets[iAssetsRow - 1].SecurityData.SecurityName = lastName.ToString();
                    }
                    acf.Rows[iRow][0] = lastName;
                    acf.Rows[iRow][0] = lastName;

                }
            }
        }

        private static Dictionary<int, int> SetRowSpansDic(DataTable table)
        {
            var rowSpansDic = new Dictionary<int, int>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                rowSpansDic[i] = 1;
            }

            if (table.Rows.Count < 2)
            {
                return rowSpansDic;
            }

            var categoryCatch = new List<string>();
            for (int i = table.Rows.Count - 1; i > 0; --i)
            {
                var prevRow = table.Rows[i - 1];
                var prevCategory = prevRow[0].ToString();
                var prevName = prevRow[1].ToString();

                var row = table.Rows[i];
                var category = row[0].ToString();
                var name = row[1].ToString();

                if (category == prevCategory
                    && !categoryCatch.Any(x => x == prevCategory + "|" + prevName))
                {
                    rowSpansDic[i - 1] += rowSpansDic[i];
                    rowSpansDic[i] = 0;
                    categoryCatch.Add(category + "|" + name);
                }
                else
                {
                    categoryCatch.Clear();
                }
            }

            return rowSpansDic;
        }

        private static void SplitCategoryByNote(DataTable cf)
        {
            var keywords = new[]
            {
                ".Beginning Principal Outstanding",
                ".Principal Received",
                ".Beginning Interest Due",
                ".Interest Received",
                ".Deferred Interest Received",
                ".Interest Deferred",
                ".Equity Distribution",
                ".CsdcFee"
            };

            foreach (var keyword in keywords)
            {
                int rowIndex = -1;
                while ((rowIndex = cf.IndexOfRow(
                    x => x[0].ToString() == "Notes"
                    && x[1].ToString().Contains(keyword))) != -1)
                {
                    var cellVal = cf.Rows[rowIndex][1].ToString();
                    var note = cellVal.Replace(keyword, "");
                    cf.Rows[rowIndex][0] = (note == "Sub" ? "次级" : ("优先级" + note));
                    cf.Rows[rowIndex][1] = Translate(keyword.Substring(1));
                }
            }
        }

        private static string Translate(string content)
        {
            var dict = new Dictionary<String, string> {
                { "Beginning Principal Outstanding", "期初本金" },
                { "Principal Received", "本金支付" },
                { "Beginning Interest Due", "预计利息金额" },
                { "Interest Received", "收到的利息" },
                { "Deferred Interest Received", "收到逾期利息" },
                { "Interest Deferred", "未偿利息" },
                { "Equity Distribution", "次级收入" },
            };
            if (dict.ContainsKey(content))
            {
                return dict[content];
            }

            return content;
        }

        private static NancyStaticAnalysisResult GetNancyStaticAnalysisResult(ABSDeal absDeal)
        {
            //TODO: 不同参数的设置

            var result = m_nancy.RunStaticResultByPath(
                absDeal.Setting.Cdr.ToString(),
                absDeal.Setting.Cpr.ToString(),
                absDeal.Location.YmlFolder,
                absDeal.Location.DsFolder);

            return result;
        }

        /// <summary>
        /// 对nancy返回的结果执行特殊的规则处理：
        /// 1、证券利随本清
        /// 2、证券和产品偿付频率错配
        /// 3、合并留存金额
        /// 4、自动处理中证登手续费
        /// </summary>
        /// <param name="result"></param>
        /// <param name="isCsdcDeal"></param>
        private static void RunSpecialRules(DataTable cf, bool isCsdcDeal)
        {
            //处理“证券利随本清”
            var rowIndex = cf.IndexOfRow(x => x.ItemArray[1].Equals("证券利随本清"));
            if (rowIndex >= 0)
            {
                var key = ".Receiver Distribution";
                var rowIndexReceiver = -1;
                while ((rowIndexReceiver = cf.IndexOfRow(x => x.ItemArray[1].ToString().Contains(key))) >= 0)
                {
                    var description = cf.Rows[rowIndexReceiver].ItemArray[1].ToString();
                    var noteName = description.Substring(0, description.Length - key.Length);
                    var rowIndexInterestReceived = cf.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Interest Received");
                    var rowIndexBeginningInterestDue = cf.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Beginning Interest Due");
                    if (rowIndexInterestReceived < 0 || rowIndexBeginningInterestDue < 0)
                    {
                        break;
                    }

                    //把Receiver Distribution和Interest Received的值加起来
                    for (int i = 2; i < cf.Columns.Count; ++i)
                    {
                        var totalInterest = double.Parse(cf.Rows[rowIndexInterestReceived][i].ToString())
                            + double.Parse(cf.Rows[rowIndexReceiver][i].ToString());

                        cf.Rows[rowIndexReceiver][i] = totalInterest.ToString("n2");
                    }

                    cf.CopyRow(rowIndexReceiver, rowIndexInterestReceived, 2);
                    cf.CopyRow(rowIndexReceiver, rowIndexBeginningInterestDue, 2);
                    cf.Rows.RemoveAt(rowIndexReceiver);
                }
            }

            //备份第一种处理“证券利随本清”的方法
            //rowIndex = cf.IndexOfRow(x => x.ItemArray[1].Equals("Alpha证券利随本清Alpha"));
            //if (rowIndex >= 0)
            //{
            //    cf.Rows.RemoveAt(rowIndex);

            //    //收到的逾期利息
            //    var key = ".Deferred Interest Received";
            //    var rowIndexDeferredInterestReceived = -1;
            //    while ((rowIndexDeferredInterestReceived = cf.IndexOfRow(x => x.ItemArray[1].ToString().Contains(key))) >= 0)
            //    {
            //        var description = cf.Rows[rowIndexDeferredInterestReceived].ItemArray[1].ToString();
            //        var noteName = description.Substring(0, description.Length - key.Length);

            //        //收到的利息
            //        var rowIndexInterestReceived = cf.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Interest Received");
            //        //预计收到的利息
            //        var rowIndexBeginningInterestDue = cf.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Beginning Interest Due");
            //        //未偿利息（逾期利息）
            //        var rowIndexInteststDeferred = cf.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Interest Deferred");
            //        if (rowIndexInterestReceived < 0 || rowIndexBeginningInterestDue < 0 || rowIndexInterestReceived < 0)
            //        {
            //            break;
            //        }

            //        for (int i = 2; i < cf.Columns.Count; ++i)
            //        {
            //            var interestReceived = double.Parse(cf.Rows[rowIndexInterestReceived][i].ToString())
            //                + double.Parse(cf.Rows[rowIndexDeferredInterestReceived][i].ToString());

            //            //收到的利息=收到的利息+收到的逾期利息
            //            cf.Rows[rowIndexInterestReceived][i] = interestReceived.ToString("n2");
            //            //预计收到的利息=收到的利息+收到的逾期利息
            //            cf.Rows[rowIndexBeginningInterestDue][i] = cf.Rows[rowIndexInterestReceived][i];
            //        }

            //        //清空未偿利息（逾期利息）
            //        cf.Rows.RemoveAt(rowIndexInteststDeferred);
            //        //清空收到的逾期利息
            //        cf.Rows.RemoveAt(rowIndexDeferredInterestReceived);
            //    }
            //}

            var dsTable = new DsTable(cf);
            //处理“证券和产品偿付频率错配”
            MergeReceiverDistributionWithInterestReceived(dsTable);

            //合并留存金额
            MergeReserveRows(dsTable);

            dsTable.OverrideTo(cf);

            //是否自动处理中证手续费
            if (isCsdcDeal)
            {
                var csdcRows = new List<Tuple<int, DataRow>>();

                var key = ".Interest Received";
                for (var i = 0; i < cf.Rows.Count; ++i)
                {
                    var row = cf.Rows[i];
                    if (row.ItemArray[1].ToString().EndsWith(key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var description = row.ItemArray[1].ToString();
                        var noteName = description.Substring(0, description.Length - key.Length);

                        var principalReceivedIndex = cf.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Principal Received");

                        var csdcRow = cf.NewRow();
                        csdcRow[0] = "Notes";
                        csdcRow[1] = noteName + ".CsdcFee";

                        for (int j = 2; j < cf.Columns.Count; ++j)
                        {
                            //中证登手续费 = (本金 + 利息) * 万分之0.5
                            var totalReceived = double.Parse(cf.Rows[principalReceivedIndex][j].ToString())
                                + double.Parse(row[j].ToString());

                            csdcRow[j] = (totalReceived * 0.00005).ToString("n2");
                        }

                        csdcRows.Add(Tuple.Create(i, csdcRow));
                    }
                }

                csdcRows = csdcRows.OrderByDescending(x => x.Item1).ToList();

                foreach (var csdcRow in csdcRows)
                {
                    cf.Rows.InsertAt(csdcRow.Item2, csdcRow.Item1);
                }
            }
        }

        /// <summary>
        /// 合并留存金额
        /// </summary>
        private static void MergeReserveRows(DsTable dsTable)
        {
            var sumReserveRow = new DsRow();
            for (int i = 0; i < dsTable.ColumnCount - 2; i++)
            {
                sumReserveRow.AppendValue((0m).ToString("n2"));
            }

            var rowCsdcReserve = dsTable.FindRow("中证本息兑付留存.End Balance");
            if (rowCsdcReserve != null)
            {
                sumReserveRow += rowCsdcReserve;
            }

            var rows = dsTable.Rows.Where(x => x.Description.EndsWith("Reserve.End Balance", StringComparison.CurrentCultureIgnoreCase)
                || x.Description.EndsWith("留存账户.End Balance", StringComparison.CurrentCultureIgnoreCase)).ToList();

            foreach (var row in rows)
            {
                sumReserveRow += row;
            }

            sumReserveRow.Name = "Reserve Accounts";
            sumReserveRow.Description = "当期留存账户余额";

            var reserveAccountDisplayEventRow = dsTable.FindRow("留存账户显示事件");
            if (reserveAccountDisplayEventRow == null)
            {
                dsTable.RemoveRow(rowCsdcReserve);
                dsTable.RemoveRow(rows);
            }
            else
            {
                dsTable.RemoveRow(reserveAccountDisplayEventRow);
            }

            //汇总后的留存金额插入到表中
            var insertRow = false;
            for (int i = dsTable.Rows.Count - 1; i >= 0; i--)
            {
                var row = dsTable.Rows[i];
                if (row.Name.Equals("Reserve Accounts", StringComparison.CurrentCultureIgnoreCase))
                {
                    dsTable.Rows.Insert(i + 1, sumReserveRow);
                    insertRow = true;
                    break;
                }
            }

            if (!insertRow)
            {
                dsTable.Rows.Add(sumReserveRow);
            }
        }

        /// <summary>
        /// 证券和产品偿付频率错配
        /// 除了次级证券以外，把 Receiver Distribution 和 Interest Received 的值求和后
        /// 覆盖掉 Interest Received 和 Beginning Interest Due
        /// 删除掉 Receiver Distribution
        /// </summary>
        private static void MergeReceiverDistributionWithInterestReceived(DsTable dsTable)
        {
            if (dsTable.Rows.Any(x => x.Description == "证券和产品偿付频率错配"))
            {
                var notes = dsTable.Notes.Where(x => !x.Equals("Sub", StringComparison.CurrentCultureIgnoreCase));
                foreach (var note in notes)
                {
                    var rowReceiverDistribution = dsTable.FindRow(note + ".Receiver Distribution");
                    var rowInterestReceived = dsTable.FindRow(note + ".Interest Received");
                    if (rowReceiverDistribution != null && rowInterestReceived != null)
                    {
                        var sumRow = rowReceiverDistribution + rowInterestReceived;
                        rowInterestReceived.CopyValueFrom(sumRow);

                        var rowBeginningInterestDue = dsTable.FindRow(note + ".Beginning Interest Due");
                        if (rowBeginningInterestDue != null)
                        {
                            rowBeginningInterestDue.CopyValueFrom(sumRow);
                        }
                    }

                    dsTable.RemoveRow(rowReceiverDistribution);
                }
            }
        }

        public static void UpdateDatasetByPredictModel(ProjectLogicModel project)
        {
            var dbAdapter = new DBAdapter();

            var dealSchedule = project.DealSchedule.Instanse;

            var errorMsg = "Error occurred in UpdateDatasetByPredictModel";
            CommUtils.Assert(dealSchedule.DeterminationDates != null && dealSchedule.DeterminationDates.Length > 0, errorMsg);
            CommUtils.Assert(dealSchedule.PaymentDates != null && dealSchedule.PaymentDates.Length > 0, errorMsg);
            CommUtils.Assert(dealSchedule.DeterminationDates.Length == dealSchedule.PaymentDates.Length, errorMsg);

            var asOfDateEndList = dealSchedule.DeterminationDates.ToList();

            var asOfDateBeginList = asOfDateEndList.Select(x => x).ToList();
            asOfDateBeginList.RemoveAt(asOfDateBeginList.Count - 1);
            asOfDateBeginList.Insert(0, dealSchedule.FirstCollectionPeriodStartDate);

            var newAsOfDateList = project.EnablePredictMode ? asOfDateBeginList : asOfDateEndList;
            var oldAsOfDateList = project.EnablePredictMode ? asOfDateEndList : asOfDateBeginList;
            var paymentDateList = dealSchedule.PaymentDates.ToList();

            var durationPeriods = new List<DatasetScheduleLogicModel>();
            if (project.EnablePredictMode)
            {
                //切换到预测模式
                //使用过去一期的模型collateral里asset的PrincipalBalance覆盖掉未来一期的值
                //从过去到未来遍历
                durationPeriods.AddRange(project.DealSchedule.DurationPeriods);
            }
            else
            {
                //切换到当期模式：
                //使用未来一期的模型collateral里asset的PrincipalBalance覆盖掉过去一期的值
                //从未来到过去遍历
                durationPeriods.AddRange(project.DealSchedule.DurationPeriods);
                durationPeriods.Reverse();
            }

            //Fields: asOfDate, assetId, principalBalance
            foreach (var period in durationPeriods)
            {
                var dataset = period.Dataset.Instance;
                if (dataset == null)
                {
                    continue;
                }

                var idx = paymentDateList.IndexOf(period.PaymentDate);
                if (idx < 0 || idx >= newAsOfDateList.Count)
                {
                    continue;
                }

                var ymlFolder = dbAdapter.Dataset.GetYmlFolder(project.Instance);

                var newAsOfDate = newAsOfDateList[idx];
                var newDatasetFolder = dbAdapter.Dataset.GetDatasetFolder(project.Instance, newAsOfDate);
                var newCollateralCsvPath = Path.Combine(newDatasetFolder, "collateral.csv");

                var oldAsOfDate = oldAsOfDateList[idx];
                var oldDatasetFolder = dbAdapter.Dataset.GetDatasetFolder(project.Instance, oldAsOfDate);
                var oldCollateralCsvPath = Path.Combine(oldDatasetFolder, "collateral.csv");

                dataset.AsOfDate = DateUtils.DateToDigitString(newAsOfDate);
                dbAdapter.Dataset.Update(dataset);

                if (File.Exists(newCollateralCsvPath))
                {
                    var assetPrincipalBalance = new Dictionary<int, string>();
                    if (File.Exists(oldCollateralCsvPath))
                    {
                        var oldCollateralCsv = new CollateralCsv();
                        oldCollateralCsv.Load(oldCollateralCsvPath);
                        foreach (var record in oldCollateralCsv)
                        {
                            //保存当次遍历Asset的期初本金
                            assetPrincipalBalance[record.AssetId] = record.Items["PrincipalBalance"];
                        }
                    }
                    else
                    {
                        if (oldAsOfDate > newAsOfDate && period.Next != null)
                        {
                            var deal = new ABSDeal(ymlFolder, newDatasetFolder);
                            var acfDataset = deal.Result.AcfResult.Dataset.FirstOrDefault(x =>
                                x.PaymentDay == period.Previous.PaymentDate);
                            if (acfDataset != null)
                            {
                                assetPrincipalBalance = acfDataset.ToDictionary(x => x.Asset.Id, x => x.Performing.ToString("n2"));
                            }
                        }
                    }

                    var collateralAsOfDate = asOfDateBeginList[idx];

                    var newCollateralCsv = new CollateralCsv();
                    newCollateralCsv.Load(newCollateralCsvPath);

                    foreach (var record in newCollateralCsv)
                    {
                        var assetId = record.AssetId;
                        //更新封包日日期
                        newCollateralCsv.UpdateCellValue(assetId, "AsOfDate",
                            collateralAsOfDate.ToString("MM/dd/yyyy"));

                        //更新Asset的期初本金
                        if (assetPrincipalBalance.ContainsKey(assetId))
                        {
                            newCollateralCsv.UpdateCellValue(assetId, "PrincipalBalance",
                                assetPrincipalBalance[assetId]);
                        }
                    }
                    newCollateralCsv.Save(newCollateralCsvPath);
                }
            }
        }

        public static DealSchedule GetNancyDealSchedule(ABSDeal absDeal)
        {
            return m_nancy.GetDealSchedule(absDeal.Location.Yml, absDeal.Location.DsFolder);
        }

        public class NancyDealInfo
        {
            public NancyDealData Info { get; set; }
            public DateTime? FirstCollectionPeriodStartDate { get; set; }
            public bool UseCustomCashflow { get; set; }
        }

        public static NancyDealInfo GetNancyDealData(ABSDeal absDeal)
        {
            using (StreamReader sr = new StreamReader(absDeal.Location.Yml))
            {
                var dateFromYml = NancyUtils.ReadYmlExInfo(sr);
                var info = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                info.UseCustomCashflow = dateFromYml.Item2;

                return new NancyDealInfo
                {
                    Info = info,
                    FirstCollectionPeriodStartDate = dateFromYml.Item1,
                    UseCustomCashflow = dateFromYml.Item2,
                };
            }
        }

        private static NancyAbsProxyService m_nancy = new NancyAbsProxyService();
    }
}
