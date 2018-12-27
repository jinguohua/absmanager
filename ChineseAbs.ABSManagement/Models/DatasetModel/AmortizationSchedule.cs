using ChineseAbs.ABSManagement.AssetEvent;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class AmortizationSchedule : List<AmortizationScheduleRecord>
    {
        private string m_filePath;

        public void Load(string filePath)
        {
            Clear();

            if (!File.Exists(filePath))
            {
                return;
            }

            var dt = ExcelUtils.ReadCsv(filePath);

            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                var row = dt.Rows[i];
                if (row.ItemArray.Length < 3)
                {
                    continue;
                }
                
                int assetId;
                DateTime reductionDate;
                double reductionAmount;
                if (int.TryParse(row.ItemArray[0].ToString(), out assetId)
                    && DateTime.TryParse(row.ItemArray[1].ToString(), out reductionDate)
                    && double.TryParse(row.ItemArray[2].ToString(), out reductionAmount)
                    && MathUtils.MoneyNE(reductionAmount, 0))
                {
                    var record = new AmortizationScheduleRecord()
                    {
                        AssetId = assetId,
                        ReductionDate = reductionDate,
                        ReductionAmount = reductionAmount
                    };

                    Add(record);
                }
            }

            m_filePath = filePath;
        }

        public List<AmortizationScheduleRecord> SelectByAsset(int assetId)
        {
            return this.Where(x => x.AssetId == assetId && MathUtils.MoneyNE(x.ReductionAmount, 0)).OrderBy(x => x.ReductionDate).ToList();
        }

        public void Save(string filePath = null)
        {
            DataTable dtAmort = new DataTable();
            dtAmort.Columns.Add("AssetId");
            dtAmort.Columns.Add("ReductionDate");
            dtAmort.Columns.Add("ReductionAmount");

            var list = this.OrderBy(x => x.AssetId).ThenBy(x => x.ReductionDate);
            foreach (var record in list)
            {
                dtAmort.Rows.Add(record.AssetId, record.ReductionDate.ToString("yyyy/MM/dd"), record.ReductionAmount.ToString("n2"));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = m_filePath;
            }

            ExcelUtils.WriteCsv(dtAmort, filePath);
        }

        public void AddPrepayment(int assetId, DateTime prepayDate, double money,
            PrepayDistrubutionType distributionType, string distributionDetail = null)
        {
            var preRecords = this.Where(x => x.AssetId == assetId && x.ReductionDate <= prepayDate)
                .OrderBy(x => x.ReductionDate).ToList();

            var records = this.Where(x => x.AssetId == assetId && x.ReductionDate > prepayDate)
                .OrderBy(x => x.ReductionDate).ToList();


            var sumMoney = records.Sum(x => x.ReductionAmount);
            CommUtils.Assert(MathUtils.MoneyGTE(sumMoney, money),
                "{0}之后(不含)的未偿还金额 {1} 少于提前偿付金额 {2}",
                prepayDate.ToShortDateString(), sumMoney.ToString("n2"), money.ToString("n2"));

            double reducingMoney = money;
            Action<AmortizationScheduleRecord> reduceByOrder = x =>
            {
                if (x.ReductionAmount >= reducingMoney)
                {
                    x.ReductionAmount -= reducingMoney;
                    reducingMoney = 0;
                }
                else
                {
                    reducingMoney -= x.ReductionAmount;
                    x.ReductionAmount = 0;
                }
            };

            switch (distributionType)
            {
                case PrepayDistrubutionType.CurrentToLast:
                    records.ForEach(reduceByOrder);
                    break;
                case PrepayDistrubutionType.LastToCurrent:
                    records.Reverse();
                    records.ForEach(reduceByOrder);
                    records.Reverse();
                    break;
                case PrepayDistrubutionType.Average:
                    var deltaMoney = money / records.Count;
                    if (records.Any(x => x.ReductionAmount < deltaMoney))
                    {
                        var record = records.First(x => x.ReductionAmount < deltaMoney);
                        CommUtils.Assert(false, "平均分配失败，第 [" + record.ReductionDate.ToString("yyyy-MM-dd") + "] 期偿付金额 ["
                            + record.ReductionAmount.ToString("n2") + "] 少于平均分配金额 [" + deltaMoney.ToString("n2") + "] 。");
                    }
                    records.ForEach(x => x.ReductionAmount -= deltaMoney);
                    break;
                case PrepayDistrubutionType.EqualRatio:
                    var percent = (sumMoney - money) / sumMoney;
                    records.ForEach(x => x.ReductionAmount *= percent);
                    break;
                case PrepayDistrubutionType.Custom:
                    CommUtils.Assert(!string.IsNullOrEmpty(distributionDetail), "没有自定义偿付信息。");

                    var oldSumMoney = preRecords.Sum(x => x.ReductionAmount)
                        + records.Sum(x => x.ReductionAmount);

                    records = ParseDistributionDetail(assetId, distributionDetail);

                    var newSumMoney = records.Sum(x => x.ReductionAmount);
                    CommUtils.Assert(MathUtils.MoneyEQ(oldSumMoney, newSumMoney + money), "自定义偿付金额 [" + (newSumMoney + money).ToString("n2")
                        + "] 和原偿付金额 [" + oldSumMoney.ToString("n2") + "] 不相等。");

                    preRecords.Clear();
                    break;
            }

            var newReductionAmount = money;
            //整合支付日期相同的数据
            if (preRecords.Any(x => x.ReductionDate == prepayDate))
            {
                var sum = preRecords.Where(x => x.ReductionDate == prepayDate).Sum(x => x.ReductionAmount);
                newReductionAmount += sum;
                preRecords.RemoveAll(x => x.ReductionDate == prepayDate);
            }

            if (records.Any(x => x.ReductionDate == prepayDate))
            {
                var sum = records.Where(x => x.ReductionDate == prepayDate).Sum(x => x.ReductionAmount);
                newReductionAmount += sum;
                records.RemoveAll(x => x.ReductionDate == prepayDate);
            }

            records.Insert(0, new AmortizationScheduleRecord
            {
                AssetId = assetId,
                ReductionAmount = newReductionAmount,
                ReductionDate = prepayDate
            });

            records.InsertRange(0, preRecords);

            this.RemoveAll(x => x.AssetId == assetId || MathUtils.Equals(x.ReductionAmount, 0));
            this.AddRange(records);
            this.Sort((l, r) => l.ReductionDate.CompareTo(r.ReductionDate));
        }

        private List<AmortizationScheduleRecord> ParseDistributionDetail(int assetId, string distributionDetail)
        {
            var records = new List<AmortizationScheduleRecord>();
            var items = distributionDetail.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in items)
            {
                var subStrs = item.Split('|');
                CommUtils.AssertEquals(subStrs.Length, 2, "解析自定义偿付信息失败[" + distributionDetail + "]");
                records.Add(new AmortizationScheduleRecord
                {
                    AssetId = assetId,
                    ReductionDate = DateUtils.ParseDigitDate(subStrs[0]),
                    ReductionAmount = double.Parse(subStrs[1])
                });
            }

            return records;
        }
    }
}
