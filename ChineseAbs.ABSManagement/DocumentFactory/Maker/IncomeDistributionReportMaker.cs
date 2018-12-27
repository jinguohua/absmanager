using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.DocumentFactory.Maker
{
    public class IncomeDistributionReportMaker : DocumentMakerBase
    {
        public IncomeDistributionReportMaker(string userName)
            :base(userName)
        {
        }

        protected override string GetPatternFilePath()
        {
            return DocumentPattern.GetPath(m_project, DocPatternType.IncomeDistributionReport);
        }

        protected override object MakeObjectInstance()
        {
            var logicModel = new ProjectLogicModel(m_userName, m_project);

            var schedule = logicModel.DealSchedule.Instanse;

            var firstNoteAccrualDates = schedule.NoteAccrualDates.First().Value;
            var firstNoteName = schedule.NoteAccrualDates.First().Key;

            foreach (var key in schedule.NoteAccrualDates.Keys)
            {
                var noteAccrualDates = schedule.NoteAccrualDates[key];
                CommUtils.AssertEquals(firstNoteAccrualDates.Length, noteAccrualDates.Length,
                    "检测到证券期数不一致，[{0}]={1},[{2}]={3}",
                    firstNoteName, firstNoteAccrualDates.Length,
                    key, noteAccrualDates.Length);

                for (int i = 0; i < firstNoteAccrualDates.Length; i++)
                {
                    CommUtils.Assert(firstNoteAccrualDates[i] == noteAccrualDates[i],
                        "检测到第[{0}]期证券Accrual date不一致，[{1}]={2}，[{3}]={4}",
                        i + 1, firstNoteName, firstNoteAccrualDates[i].ToShortDateString(),
                        key, noteAccrualDates[i].ToShortDateString());
                }
            }

            //从第N期开始模型数据时，getDealSchedule中不包含前几期的PaymentDate
            List<DateTime> paymentDates = schedule.PaymentDates.ToList();
            if (m_project.CnabsDealId.HasValue)
            {
                paymentDates = m_dbAdapter.Model.GetPaymentDates(m_project.CnabsDealId.Value);
            }

            var datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(m_project.ProjectId);
            var paymentDate = paymentDates.First(x => x == m_paymentDay);
            var sequence = paymentDates.FindIndex(x => x == m_paymentDay);
            datasets = datasets.Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value <= paymentDate).ToList();
            var findDatasets = datasets.Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value == paymentDate).ToList();
            findDatasets.Sort((l, r) => l.AsOfDate.CompareTo(r.AsOfDate));
            CommUtils.Assert(findDatasets.Count >= 1, "找不到偿付期为 [{0}] 的数据模型", DateUtils.DateToString(paymentDate));
            var dataset = findDatasets[0];
            var datasetId = dataset.DatasetId;

            var notes = m_dbAdapter.Dataset.GetNotes(m_project.ProjectId);
            var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(datasetId);

            //初始化note信息
            List<Note> cnabsNotes = new ProjectLogicModel(m_userName, m_project).Notes;

            
            var datasetFolder = m_dbAdapter.Dataset.GetDatasetFolder(m_project, dataset.AsOfDate);
            var variablesCsv = new VariablesHelper(datasetFolder);
            var futureVariablesPath = Path.Combine(datasetFolder, "FutureVariables.csv");
            var variables = variablesCsv.GetVariablesByDate(dataset.PaymentDate.Value);
            var rateResetRecords = InterestRateUtils.RateResetRecords(variables);

            var idrObj = new IncomeDistributionReport();
            idrObj.Sequence = sequence + 1;
            idrObj.SequenceCN = idrObj.Sequence.ToCnString();
            idrObj.Security = new Dictionary<string, PaymentDetail>();
            idrObj.PriorSecurityList = new List<PaymentDetail>();
            idrObj.SubSecurityList = new List<PaymentDetail>();
            idrObj.SecurityList = new List<PaymentDetail>();
            idrObj.BeginAccrualDate = sequence == 0 ? schedule.FirstAccrualDate : schedule.NoteAccrualDates.First().Value[sequence-1];
            idrObj.EndAccrualDate = schedule.NoteAccrualDates.First().Value[sequence];
            idrObj.AccrualDateSum = (idrObj.EndAccrualDate - idrObj.BeginAccrualDate).Days;
            for (int i = 0; i < notes.Count;i++ )
            {
                var note = notes[i];
                note.CouponString = InterestRateUtils.CalculateCurrentCouponRate(cnabsNotes[i].CouponString, rateResetRecords);

                var noteData = noteDatas.Single(x => x.NoteId == note.NoteId);

                CommUtils.Assert(noteData.HasValue, "兑付日为[{0}]的偿付期内，证券端现金流类型的工作未核对", paymentDate.ToShortDateString());

                idrObj.Security[note.ShortName] = GeneratePaymentDetail(note, noteData, idrObj.Security.Count + 1);

                if (note.IsEquity)
                {
                    idrObj.SubSecurityList.Add(GeneratePaymentDetail(note, noteData, idrObj.SubSecurityList.Count + 1));
                }
                else
                {
                    idrObj.PriorSecurityList.Add(GeneratePaymentDetail(note, noteData, idrObj.PriorSecurityList.Count + 1));
                }

                idrObj.SecurityList.Add(GeneratePaymentDetail(note, noteData, idrObj.SecurityList.Count(x => x.Money != 0) + 1));
            }

            Func<IEnumerable<PaymentDetail>, PaymentDetail> sum = (values) => new PaymentDetail
            {
                Residual = values.Sum(x => x.Residual),
                Principal = values.Sum(x => x.Principal),
                Interest = values.Sum(x => x.Interest),
                Money = values.Sum(x => x.Money),
                UnitCount = values.Sum(x => x.UnitCount),
                SumPaymentAmount = values.Sum(x => x.SumPaymentAmount)
            };

            idrObj.Sum = sum(idrObj.Security.Values);
            idrObj.SumPrior = sum(idrObj.Security.Values.Where(x => !x.IsEquity));
            idrObj.SumSub = sum(idrObj.Security.Values.Where(x => x.IsEquity));

            GeneratePercentTable(idrObj, datasets, notes, schedule.PaymentDates);

            idrObj.RepayDetail = GenerateRepayDetail(idrObj.SecurityList, x => x.NameCN);
            idrObj.RepayDetailWithHyphen = GenerateRepayDetail(idrObj.SecurityList, x => x.NameCNHyphen);
            idrObj.RepayDetailWithHyphenByJinTai = GenerateRepayDetailByJinTai(idrObj.SecurityList, x => x.NameCNHyphen);
            idrObj.RepayPrincipalDetail = GenerateRepayPrincipalDetail(idrObj.SecurityList);
            idrObj.DenominationDetail = GenerateDenominationDetail(idrObj.SecurityList);
            idrObj.EquityRegisterDetail = GenerateEquityRegisterDetail(idrObj.SecurityList, dataset.PaymentDate.Value);
            idrObj.EquityRegisterDetailByJinTai = GenerateEquityRegisterDetailByJinTai(idrObj.SecurityList, dataset.PaymentDate.Value);
            idrObj.EquityRegisterDetailByZhongGang = GenerateEquityRegisterDetailByZhongGang(idrObj.SecurityList, dataset.PaymentDate.Value);
            idrObj.EquityRegisterDetailByYingBinGuan = GenerateEquityRegisterDetailByYingBinGuan(idrObj.SecurityList, dataset.PaymentDate.Value);

            idrObj.T = dataset.PaymentDate.Value;

            if (paymentDates.First() == idrObj.T)
            {
                idrObj.PreviousT = schedule.ClosingDate;
            }
            else
            {
                var previousIndex = paymentDates.IndexOf(idrObj.T) - 1;
                idrObj.PreviousT = paymentDates[previousIndex];
            }

            idrObj.DurationDayCount = (idrObj.T - idrObj.PreviousT).Days;

            var t_1 = dataset.PaymentDate.Value.AddDays(-1);
            while (!CalendarCache.IsTradingDay(t_1))
            {
                t_1 = t_1.AddDays(-1);
            }
            idrObj.T_1 = t_1;
            idrObj.Date = DateTime.Today;
            idrObj.TaskEndTime = m_timeStamp;
            return idrObj;
        }

        private PaymentDetail GeneratePaymentDetail(Note note, NoteData noteData, int sequence)
        {
            var detail = new PaymentDetail();
            detail.Sequence = sequence;
            detail.NameCN = note.NoteName;
            detail.NameCNHyphen = GenerateNameCNHyphen(note);
            detail.NameCNFullHyphen = detail.NameCNHyphen + "级资产支持证券";
            detail.NameEN = note.ShortName;
            detail.NameENHyphen = InsertHyphenBeforeNumber(detail.NameEN);
            detail.NameENUnderline = detail.NameENHyphen.Replace("-","_");
            detail.IsEquity = note.IsEquity;
            detail.Residual = noteData.EndingBalance.Value + noteData.PrincipalPaid.Value;
            detail.Notional = note.Notional.Value;
            detail.UnitCount = (int)(note.Notional.Value / 100);
            detail.Principal = noteData.PrincipalPaid.Value;
            detail.Interest = noteData.InterestPaid.Value;
            detail.Money = detail.Principal + detail.Interest;
            detail.UnitPrincipal = detail.Principal / detail.UnitCount;
            detail.UnitInterest = detail.Interest / detail.UnitCount;
            detail.UnitMoney = detail.Money / detail.UnitCount;
            detail.Denomination = noteData.EndingBalance.Value / note.Notional.Value * 100;
            detail.CouponString = note.CouponString.Replace(" ", "");
            detail.PrincipalPercent = detail.Notional == 0 ? 0 : detail.Principal / detail.Notional;
            detail.PrincipalPercentInDataset = detail.Residual == 0 ? 0 : detail.Principal / detail.Residual;
            detail.EndingBalance = detail.Residual - detail.Principal;
            detail.SumPaymentAmount = detail.Interest + detail.Principal;
            return detail;
        }

        /// <summary>
        /// 生成权益登记日段落（白鹭2016-1专用）
        /// </summary>
        /// <param name="paymentDeatil"></param>
        /// <returns></returns>
        private string GenerateEquityRegisterDetail(List<PaymentDetail> paymentDeatil, DateTime t)
        {
            var t_3Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal == x.Residual).ToList()
                .ConvertAll(x => x.NameCNHyphen).ToArray();
            var contentT_3 = string.Join("、", t_3Names);
            if (!string.IsNullOrEmpty(contentT_3))
            {
                var t_3 = t;
                for (int i = 0; i < 3; ++i)
                {
                    t_3 = DateUtils.GetPreviousWorkingDay(t_3);
                }
                
                contentT_3 += "的权益登记日均为" + t_3.ToString("yyyy年M月d日") + "。";
            }

            var t_1Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal != x.Residual).ToList()
                .ConvertAll(x => x.NameCNHyphen).ToArray();
            var contentT_1 = string.Join("、", t_1Names);
            if (!string.IsNullOrEmpty(contentT_1))
            {
                var t_1 = DateUtils.GetPreviousWorkingDay(t);
                contentT_1 += "的权益登记日均为" + t_1.ToString("yyyy年M月d日") + "。";
            }

            return contentT_3 + contentT_1;
        }

        /// <summary>
        /// 生成权益登记日段落（金泰专用）
        /// </summary>
        /// <param name="paymentDeatil"></param>
        /// <returns></returns>
        private string GenerateEquityRegisterDetailByJinTai(List<PaymentDetail> paymentDeatil, DateTime t)
        {
            var t_1Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal != x.Residual).ToList()
                .ConvertAll(x => x.NameCNFullHyphen).ToArray();
            var contentT_1 = CommUtils.Join("、", "和", t_1Names.ToList());
            if (!string.IsNullOrEmpty(contentT_1))
            {
                var t_1 = DateUtils.GetPreviousWorkingDay(t);
                contentT_1 += "的权益登记日均为" + t_1.ToString("yyyy年M月d日") + "。";
            }

            return contentT_1;
        }

        /// <summary>
        /// 生成权益登记日段落（中港专用）
        /// </summary>
        /// <param name="paymentDeatil"></param>
        /// <returns></returns>
        private string GenerateEquityRegisterDetailByZhongGang(List<PaymentDetail> paymentDeatil, DateTime t)
        {
            var t_1Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal != x.Residual).ToList()
                .ConvertAll(x => x.NameCN).ToArray();
            var contentT_1 = CommUtils.Join("、", "和", t_1Names.ToList());
            if (!string.IsNullOrEmpty(contentT_1))
            {
                var t_1 = DateUtils.GetPreviousWorkingDay(t);
                contentT_1 += "的权益登记日均为" + t_1.ToString("yyyy年M月d日") + "。";
            }

            return contentT_1;
        }

        /// <summary>
        /// 生成权益登记日段落（迎宾馆2016-1专用）
        /// </summary>
        /// <param name="paymentDeatil"></param>
        /// <returns></returns>
        private string GenerateEquityRegisterDetailByYingBinGuan(List<PaymentDetail> paymentDeatil, DateTime t)
        {
            var t_3Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal == x.Residual).ToList()
                .ConvertAll(x => x.NameCN).ToArray();
            var contentT_3 = CommUtils.Join("、", "和", t_3Names.ToList());
            if (!string.IsNullOrEmpty(contentT_3))
            {
                var t_3 = t;
                for (int i = 0; i < 3; ++i)
                {
                    t_3 = DateUtils.GetPreviousWorkingDay(t_3);
                }

                contentT_3 += "的权益登记日均为" + t_3.ToString("yyyy年M月d日") + "。";
            }

            var t_1Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal != x.Residual).ToList()
                .ConvertAll(x => x.NameCN).ToArray();
            var contentT_1 = CommUtils.Join("、", "和", t_1Names.ToList());
            if (!string.IsNullOrEmpty(contentT_1))
            {
                var t_1 = DateUtils.GetPreviousWorkingDay(t);
                contentT_1 += "的权益登记日均为" + t_1.ToString("yyyy年M月d日") + "。";
            }

            return contentT_3 + contentT_1;
        }

        private string GenerateDenominationDetail(List<PaymentDetail> paymentDeatil)
        {
            var sentences = paymentDeatil.Where(x => x.Denomination != 100).ToList().ConvertAll(x =>
            {
                var text = string.Empty;
                text += (x.IsEquity ? "次级" : "优先" + x.NameENHyphen + "级");
                text += "资产支持证券每份面值为" + x.Denomination.ToString("n2") + "元";
                return text;
            });

            var content = string.Empty;
            content = string.Join("，", sentences.ToArray());
            if (!string.IsNullOrEmpty(content))
            {
                content += (paymentDeatil.Exists(x => x.Denomination == 100) ? "，" : "。");
            }

            sentences = paymentDeatil.Where(x => x.Denomination == 100).ToList().ConvertAll(x =>
            {
                return (x.IsEquity ? "次级" : "优先" + x.NameENHyphen + "级");
            });

            var sentence100 = string.Join("、", sentences.ToArray());
            if (!string.IsNullOrEmpty(sentence100))
            {
                sentence100 += "资产支持证券每份面值为100元。";
            }

            return content + sentence100;
        }

        private string GenerateRepayPrincipalDetail(List<PaymentDetail> paymentDeatil)
        {
            var sentences = paymentDeatil.Where(x => x.Notional != x.Residual).ToList().ConvertAll(x =>
            {
                var text = string.Empty;
                text += (x.IsEquity ? "次级" : "优先" + x.NameENHyphen + "级");
                text += "截至本收益分配报告日已分期偿还本金";
                text += ((x.Notional - x.Residual) / x.Notional).ToString("p2");
                return text;
            });

            var content = string.Empty;
            content = string.Join("；", sentences.ToArray()) + "。";
            if (paymentDeatil.Exists(x => x.Notional == x.Residual))
            {
                content += "其余证券截至本收益分配报告日尚未进行还本。";
            }
            return content;
        }

        /// <summary>
        /// 生成分期偿还情况的段落（使用带-的券名）(金泰专用)
        /// </summary>
        /// <param name="paymentDetailList"></param>
        /// <param name="getName"></param>
        /// <returns></returns>
        private string GenerateRepayDetailByJinTai(List<PaymentDetail> paymentDetailList, Func<PaymentDetail, string> getName)
        {
            var listTrading = new List<string>();//本金兑付中
            var listLastTrade = new List<string>();//最后一次兑付本金
            var repaymentPercentage = new List<string>();//偿还本金百分数
            var residueMoney = new List<string>();//剩余本金面值

            foreach (var detail in paymentDetailList)
            {
                if (!detail.IsEquity)
                {
                    if (detail.Residual - detail.Principal > 0)
                    {
                        listTrading.Add(getName(detail) + "级资产支持证券");
                        repaymentPercentage.Add((detail.Principal / detail.Notional).ToString("0.00%"));
                        residueMoney.Add(((detail.Residual - detail.Principal)*100 / detail.Notional).ToString("n3"));
                    }
                    else if (detail.Residual - detail.Principal == 0 && detail.Principal > 0)
                    {
                        listLastTrade.Add(getName(detail) + "级资产支持证券");
                    }
                }
            }

            var content = string.Empty;
            if (listLastTrade.Count != 0)
            {
                content += string.Join("、", listLastTrade.ToArray()) + "经本次分配后，本金兑付完毕。";
            }
            if (listTrading.Count != 0)
            {
                content += string.Join("、", listTrading.ToArray()) + "经本次分配后，本金未兑付完毕，将继续进行交易。本次兑付，";
                for (int i = 0; i < listTrading.Count; i++)
                {
                    var textStr = double.Parse(repaymentPercentage[i].Replace("%", "").Trim()) == 0 ? "不变。" : "调整为" + residueMoney[i] + "元。";

                    content += listTrading[i] + "将偿还本金" + repaymentPercentage[i];
                    content += "，剩余本金面值" + textStr;
                }
            }
            if (content == string.Empty)
            {
                content = "本金全部兑付完毕。";
            }

            return content;
        }
        private string GenerateRepayDetail(List<PaymentDetail> paymentDetailList, Func<PaymentDetail, string> getName)
        {
            var listTrading = new List<string>();//本金兑付中
            var listLastTrade = new List<string>();//最后一次兑付本金

            foreach (var detail in paymentDetailList)
            {
                if (!detail.IsEquity)
                {
                    if (detail.Residual - detail.Principal > 0)
                    {
                        listTrading.Add(getName(detail));
                    }
                    else if (detail.Residual - detail.Principal == 0 && detail.Principal > 0)
                    {
                        listLastTrade.Add(getName(detail));
                    }
                }
            }

            var content = string.Empty;
            if (listLastTrade.Count != 0)
            {
                content += string.Join("、", listLastTrade.ToArray()) + "经本次分配后，本金兑付完毕。";
            }
            if (listTrading.Count != 0)
            {
                content += string.Join("、", listTrading.ToArray()) + "经本次分配后，本金未兑付完毕，将继续进行交易。";
            }
            if (content == string.Empty)
            {
                content = "本金全部兑付完毕。";
            }

            return content;
        }

        private void GeneratePercentTable(IncomeDistributionReport obj, List<Dataset> datasetList, List<Note> notes, DateTime[] paymentDates)
        {
            obj.PrincipalTable = new List<CashItem>();
            var rowCount = Math.Min(datasetList.Count, paymentDates.Length);
            for (int i = 0; i < datasetList.Count; ++i)
            {
                var dataset = datasetList[i];
                var cashItem = new CashItem();
                var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(dataset.DatasetId);
                cashItem.Date = dataset.PaymentDate.Value;
                cashItem.Percent = new Dictionary<string,decimal>();
                foreach (var noteData in noteDatas)
                {
                    var note = notes.Single(x => x.NoteId == noteData.NoteId);
                    CommUtils.AssertNotNull(noteData.PrincipalPaid, "请核兑付日为[{0}]期的相关数据，才能够生成本期报告", dataset.PaymentDate.Value.ToString("yyyy-MM-dd"));
                    cashItem.Percent[note.ShortName] = noteData.PrincipalPaid.Value / note.Notional.Value;
                }
                obj.PrincipalTable.Add(cashItem);
            }

            obj.PrincipalTable.Reverse();

            obj.SumPrincipalTable = new CashItem();
            obj.SumPrincipalTable.Percent = new Dictionary<string, decimal>();
            foreach (var note in notes)
            {
                obj.SumPrincipalTable.Percent[note.ShortName] = obj.PrincipalTable.Sum(x => x.Percent[note.ShortName]);
            }
        }
    }
}
