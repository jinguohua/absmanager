using ChineseAbs.ABSManagement.LogicModels.DealModel;
using ChineseAbs.ABSManagement.Utils.DatasetTable;
using ChineseAbs.CalcService.Data.NancyData;
using ChineseAbs.CalcService.Data.NancyData.Cashflows;
using ChineseAbs.Logic;
using ChineseAbs.Logic.Object;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils
{
    public class NancyUtils
    {
        private static NancyAbsProxyService m_nancy = new NancyAbsProxyService();

        public static Tuple<DateTime?, bool> ReadYmlExInfo(StreamReader sr)
        {
            var findFirstCollectionPeriodStartDate = false;
            var findUseCustomCashflow = false;
            DateTime? date = null;
            bool useCustomCashflow = false;
            while (!sr.EndOfStream)
            {
                var str = sr.ReadLine();
                if (!findFirstCollectionPeriodStartDate
                    && str.Contains("FirstCollectionPeriodStartDate:"))
                {
                    str = str.Replace("FirstCollectionPeriodStartDate:", string.Empty).Trim();
                    date = DateTime.Parse(str);
                }

                if (!findUseCustomCashflow
                    && str.Contains("UseCustomCashflow:"))
                {
                    str = str.Replace("UseCustomCashflow:", string.Empty).Trim();
                    useCustomCashflow = bool.Parse(str);
                }

                if (findUseCustomCashflow && findFirstCollectionPeriodStartDate)
                {
                    break;
                }
            }
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            return Tuple.Create(date, useCustomCashflow);
        }

        public static DealSchedule GetDealSchedule(int projectId, string asOfDate = null, bool ignoreException = true)
        {
            try
            {
                var dbAdapter = new DBAdapter();
                var rootFolder = WebConfigUtils.RootFolder;
                var project = dbAdapter.Project.GetProjectById(projectId);
                var modelFolder = Path.Combine(rootFolder, project.Model.ModelFolder);
                var ymlFilePath = modelFolder + @"\Script.yml";
                if (File.Exists(ymlFilePath))
                {
                    using (StreamReader sr = new StreamReader(ymlFilePath))
                    {
                        var dateFromYml = ReadYmlExInfo(sr);

                        var nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                        if (nancyDealData != null)
                        {
                            var scheduleData = nancyDealData.ScheduleData;
                            var dealSchedule = new DealSchedule();
                            //第一个偿付期的计息区间起始日期就是起息日
                            dealSchedule.FirstAccrualDate = scheduleData.ClosingDate;
                            dealSchedule.ClosingDate = scheduleData.ClosingDate;
                            dealSchedule.LegalMaturity = scheduleData.LegalMaturity;
                            dealSchedule.PaymentDates = scheduleData.PaymentSchedule.Periods.Select(x => x.PaymentDate).ToArray();
                            dealSchedule.DeterminationDates = scheduleData.PaymentSchedule.Periods.Select(x => x.DeterminationDate).ToArray();

                            dealSchedule.FirstCollectionPeriodStartDate = dateFromYml.Item1.HasValue
                                ? dateFromYml.Item1.Value : scheduleData.PaymentSchedule.Periods.First().CollectionBegin;
                            dealSchedule.FirstCollectionPeriodEndDate = scheduleData.PaymentSchedule.Periods.First().CollectionEnd;

                            var scheduledPaymentDateArray = nancyDealData.ScheduleData.PaymentSchedule.Periods.Select(x => x.ScheduledPaymentDate).ToArray();
                            dealSchedule.NoteAccrualDates = new Dictionary<string, DateTime[]>();
                            nancyDealData.Notes.ForEach(x => dealSchedule.NoteAccrualDates[x.Name] = scheduledPaymentDateArray);

                            return dealSchedule;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (ignoreException)
                {
                    return null;
                }
                else
                {
                    throw e;
                }
            }

            return null;
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
        private static void RunSpecialRules(NancyStaticAnalysisResult result, bool isCsdcDeal)
        {
            var table = result.CashflowDt;
            //处理“证券利随本清”
            var rowIndex = table.IndexOfRow(x => x.ItemArray[1].Equals("证券利随本清"));
            if (rowIndex >= 0)
            {
                var key = ".Receiver Distribution";
                var rowIndexReceiver = -1;
                while ((rowIndexReceiver = table.IndexOfRow(x => x.ItemArray[1].ToString().Contains(key))) >= 0)
                {
                    var description = table.Rows[rowIndexReceiver].ItemArray[1].ToString();
                    var noteName = description.Substring(0, description.Length - key.Length);
                    var rowIndexInterestReceived = table.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Interest Received");
                    var rowIndexBeginningInterestDue = table.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Beginning Interest Due");
                    if (rowIndexInterestReceived < 0 || rowIndexBeginningInterestDue < 0)
                    {
                        break;
                    }

                    //把Receiver Distribution和Interest Received的值加起来
                    for (int i = 2; i < table.Columns.Count; ++i)
                    {
                        var totalInterest = double.Parse(table.Rows[rowIndexInterestReceived][i].ToString())
                            + double.Parse(table.Rows[rowIndexReceiver][i].ToString());

                        table.Rows[rowIndexReceiver][i] = totalInterest.ToString("n2");
                    }

                    table.CopyRow(rowIndexReceiver, rowIndexInterestReceived, 2);
                    table.CopyRow(rowIndexReceiver, rowIndexBeginningInterestDue, 2);
                    table.Rows.RemoveAt(rowIndexReceiver);
                }
            }

            //备份第一种处理“证券利随本清”的方法
            //rowIndex = table.IndexOfRow(x => x.ItemArray[1].Equals("Alpha证券利随本清Alpha"));
            //if (rowIndex >= 0)
            //{
            //    table.Rows.RemoveAt(rowIndex);

            //    //收到的逾期利息
            //    var key = ".Deferred Interest Received";
            //    var rowIndexDeferredInterestReceived = -1;
            //    while ((rowIndexDeferredInterestReceived = table.IndexOfRow(x => x.ItemArray[1].ToString().Contains(key))) >= 0)
            //    {
            //        var description = table.Rows[rowIndexDeferredInterestReceived].ItemArray[1].ToString();
            //        var noteName = description.Substring(0, description.Length - key.Length);

            //        //收到的利息
            //        var rowIndexInterestReceived = table.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Interest Received");
            //        //预计收到的利息
            //        var rowIndexBeginningInterestDue = table.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Beginning Interest Due");
            //        //未偿利息（逾期利息）
            //        var rowIndexInteststDeferred = table.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Interest Deferred");
            //        if (rowIndexInterestReceived < 0 || rowIndexBeginningInterestDue < 0 || rowIndexInterestReceived < 0)
            //        {
            //            break;
            //        }

            //        for (int i = 2; i < table.Columns.Count; ++i)
            //        {
            //            var interestReceived = double.Parse(table.Rows[rowIndexInterestReceived][i].ToString())
            //                + double.Parse(table.Rows[rowIndexDeferredInterestReceived][i].ToString());

            //            //收到的利息=收到的利息+收到的逾期利息
            //            table.Rows[rowIndexInterestReceived][i] = interestReceived.ToString("n2");
            //            //预计收到的利息=收到的利息+收到的逾期利息
            //            table.Rows[rowIndexBeginningInterestDue][i] = table.Rows[rowIndexInterestReceived][i];
            //        }

            //        //清空未偿利息（逾期利息）
            //        table.Rows.RemoveAt(rowIndexInteststDeferred);
            //        //清空收到的逾期利息
            //        table.Rows.RemoveAt(rowIndexDeferredInterestReceived);
            //    }
            //}

            var dsTable = new DsTable(result.CashflowDt);
            //处理“证券和产品偿付频率错配”
            MergeReceiverDistributionWithInterestReceived(dsTable);

            //合并留存金额
            MergeReserveRows(dsTable);

            dsTable.OverrideTo(result.CashflowDt);

            //是否自动处理中证手续费
            if (isCsdcDeal)
            {
                var csdcRows = new List<Tuple<int, DataRow>>();

                var key = ".Interest Received";
                for (var i = 0; i < table.Rows.Count; ++i)
                {
                    var row = table.Rows[i];
                    if (row.ItemArray[1].ToString().EndsWith(key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var description = row.ItemArray[1].ToString();
                        var noteName = description.Substring(0, description.Length - key.Length);

                        var principalReceivedIndex = table.IndexOfRow(x => x.ItemArray[1].ToString() == noteName + ".Principal Received");

                        var csdcRow = table.NewRow();
                        csdcRow[0] = "Notes";
                        csdcRow[1] = noteName + ".CsdcFee";

                        for (int j = 2; j < table.Columns.Count; ++j)
                        {
                            //中证登手续费 = (本金 + 利息) * 万分之0.5
                            var totalReceived = double.Parse(table.Rows[principalReceivedIndex][j].ToString())
                                + double.Parse(row[j].ToString());

                            csdcRow[j] = (totalReceived * 0.00005).ToString("n2");
                        }

                        csdcRows.Add(Tuple.Create(i, csdcRow));
                    }
                }

                csdcRows = csdcRows.OrderByDescending(x => x.Item1).ToList();

                foreach (var csdcRow in csdcRows)
                {
                    table.Rows.InsertAt(csdcRow.Item2, csdcRow.Item1);
                }
            }
        }

        public static NancyStaticAnalysisResult GetStaticAnalyticsResult(int projectId, DataSet dsVariables = null,
            string asOfDate = null, AssetOverrideSetting assetOverrideSetting = null)
        {
            if (assetOverrideSetting == null)
            {
                assetOverrideSetting = new AssetOverrideSetting();
            }

            //if (dsVariables == null)
            //{
            //    dsVariables = GetOverridableVariables(projectId, asOfDate);
            //}

            var folderInfo = new ModelFolderInfo(projectId, asOfDate);
            //NancyMockUtils.SetOverridableVariables(dsVariables, folderInfo.DsFolder);

            var result = m_nancy.RunStaticResultByPath("0", "0", folderInfo.YmlFolder, folderInfo.DsFolder,
                assetOverrideSetting.PaymentDate.ToString("yyyy-MM-dd"),
                assetOverrideSetting.IsOverride, assetOverrideSetting.Interest, assetOverrideSetting.Principal);

            var isCsdcDeal = false;
            var ymlFilePath = Path.Combine(folderInfo.YmlFolder, "Script.yml");
            if (File.Exists(ymlFilePath))
            {
                using (StreamReader sr = new StreamReader(ymlFilePath))
                {
                    var nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                    if (nancyDealData != null)
                    {
                        isCsdcDeal = nancyDealData.IsCsdcDeal;
                    }
                }
            }

            RunSpecialRules(result, isCsdcDeal);

            return result;
        }

        /// <summary>
        /// 合并留存金额
        /// </summary>
        public static void MergeReserveRows(DsTable dsTable)
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
        public static void MergeReceiverDistributionWithInterestReceived(DsTable dsTable)
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

        public static NancyDealData GetNancyDealDataByFile(System.IO.Stream stream)
        {
            return m_nancy.GetNancyDealDataByFile(stream);
        }

        public static BasicAnalyticsData GetBasicAnalyticsData(int projectId, DataSet dsVariables = null, DateTime? asOfDate = null)
        {
            return GetBasicAnalyticsData(projectId, dsVariables,
                asOfDate.HasValue ? asOfDate.Value.ToString("yyyyMMdd") : null);
        }

        public static BasicAnalyticsData GetBasicAnalyticsData(int projectId, DataSet dsVariables = null, string asOfDate = null)
        {
        //    if (dsVariables == null)
        //    {
        //        dsVariables = GetOverridableVariables(projectId, asOfDate);
        //    }

            var folderInfo = new ModelFolderInfo(projectId, asOfDate);
            var result = Nancy_GetBasicAnalyticsData(folderInfo.YmlFolder, folderInfo.DsFolder, dsVariables);
            return result;
        }

        // 2018-11-07 追加：
        // NancyAbsProxyService::GetBasicAnalyticsData 中使用的是GET方式请求
        // 参数类型为DataSet时，序列化后会出现参数过程，请求Nancy失败的情况
        // 所以更新 GetBasicAnalyticsData 使用POST方式请求
        public static BasicAnalyticsData Nancy_GetBasicAnalyticsData(string path, string dsPath, DataSet ds)
        {
            RestRequest request = new RestRequest(Method.POST);
            request.Resource = "AbsManagement/GetBasicAnalyticsData";
            var variables = JsonConvert.SerializeObject(ds);
            request.AddParameter("variables", variables);
            request.AddParameter("path", path);
            request.AddParameter("dsPath", dsPath);
            var rt = m_nancy.Execute(request);

            BasicAnalyticsData obj = null;
            try
            {
                obj = JsonConvert.DeserializeObject<BasicAnalyticsData>(rt.ContentJson);
            }
            catch (Exception e)
            {
                throw new ApplicationException("读取Cashflow失败：" + Environment.NewLine + rt.ContentJson);
            }
            return obj;
        }

        public static NancyBasicAssetCashflow GetUnaggregateAssetCashflowByPath(int projectId, string asOfDate = null)
        {
            var folderInfo = new ModelFolderInfo(projectId, asOfDate);
            var result = m_nancy.GetUnaggregateAssetCashflowByPath(folderInfo.YmlFolder, folderInfo.DsFolder, null, 0, 0);
            return result;
        }

        public class ModelFolderInfo
        {
            public ModelFolderInfo(int projectId, string asOfDate)
            {
                DateTime? date = null;
                if (DateUtils.IsDigitDate(asOfDate))
                {
                    date = DateUtils.ParseDigitDate(asOfDate);
                }

                Init(projectId, date);
            }

            public ModelFolderInfo(int projectId, DateTime? asOfDate)
            {
                Init(projectId, asOfDate);
            }

            private void Init(int projectId, DateTime? asOfDate)
            {
                var dbAdapter = new DBAdapter();
                var project = dbAdapter.Project.GetProjectById(projectId);
                var ymlFilePath = dbAdapter.Dataset.GetYmlFilePath(project);
                YmlFolder = Path.GetDirectoryName(ymlFilePath);

                if (!asOfDate.HasValue)
                {
                    var latestDataset = dbAdapter.Dataset.GetLatestDatasetByProjectId(projectId);
                    asOfDate = DateUtils.ParseDigitDate(latestDataset.AsOfDate);
                }
                DsFolder = Path.Combine(YmlFolder, asOfDate.Value.ToString("yyyyMMdd"));
            }

            public string DsFolder { get; set; }
            public string YmlFolder { get; set; }
        }

        //public static DataSet GetOverridableVariables(int projectId, string asOfDate = null)
        //{
        //    var folderInfo = new ModelFolderInfo(projectId, asOfDate);
        //    return m_nancy.GetOverridableVariables(folderInfo.YmlFolder, folderInfo.DsFolder);
        //}

        public static NancyStaticAnalysisResult RunStaticResultByPath(string cdr, string cpr, string path, string dsPath,
            AssetOverrideSetting assetOverrideSetting = null)
        {
            if (assetOverrideSetting == null)
            {
                assetOverrideSetting = new AssetOverrideSetting();
            }

            var result =  m_nancy.RunStaticResultByPath(cdr, cpr, path, dsPath, assetOverrideSetting.PaymentDate.ToString("yyyy-MM-dd"),
                assetOverrideSetting.IsOverride, assetOverrideSetting.Interest, assetOverrideSetting.Principal);

            var isCsdcDeal = false;
            var ymlFilePath = Path.Combine(path, @"Script.yml");
            if (File.Exists(ymlFilePath))
            {
                using (StreamReader sr = new StreamReader(ymlFilePath))
                {
                    var nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                    if (nancyDealData != null)
                    {
                        isCsdcDeal = nancyDealData.IsCsdcDeal;
                    }
                }
            }

            RunSpecialRules(result, isCsdcDeal);
            return result;
        }
    }
}
