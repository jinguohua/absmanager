using ChineseAbs.ABSManagement.LogicModels.OverrideSingleAsset;
using ChineseAbs.ABSManagement.Utils;
using CNABS.Mgr.Deal.Utils;
using SFL.CDOAnalyser.MasterData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CNABS.Mgr.Deal.Model.Result
{
    /// <summary>
    /// 资产端现金流表
    /// </summary>
    public class AcfResult
    {
        public AcfResult(DataTable acf, Assets assets)
        {
            Asset = new List<AcfAsset>();
            Dataset = new List<AcfDataset>();

            Init(acf, assets);
            ReCalcSum();
        }

        private void Init(DataTable acf, Assets assets)
        {
            List<string> categoryNameCache = new List<string>();
            for (int i = 0; i < acf.Rows.Count; i++)
            {
                var row = acf.Rows[i];
                if (row.ItemArray.Length < 2)
                {
                    continue;
                }

                var category = row.ItemArray[0].ToString();
                var name = row.ItemArray[1].ToString();
                var categoryName = category + "|" + name;
                var startAssetSequence = categoryNameCache.Count(x => x == categoryName);
                categoryNameCache.Add(categoryName);
                var asset = assets.GetByName(category, startAssetSequence);
                if (asset == null)
                {
                    if (string.IsNullOrEmpty(category))
                    {
                        continue;
                    }

                    var secrutyData = new CSecurityData();
                    secrutyData.AssetId = -1;
                    secrutyData.SecurityName = category;
                    asset = new Asset(secrutyData);
                }

                for (int j = 2; j < row.ItemArray.Length; j++)
                {
                    DateTime date;
                    if (DateTime.TryParse(acf.Columns[j].ColumnName, out date))
                    {
                        AddAcfUnit(asset, date, name, row.ItemArray[j].ToString());
                    }
                }
            }
        }

        public void ReCalcSum()
        {
            foreach (var ds in Dataset)
            {
                ds.Sum.SetSumOf(ds);
            }

            foreach (var asset in Asset)
            {
                asset.Sum.SetSumOf(asset);
            }
        }

        private void AddAcfUnit(Asset asset, DateTime paymentDay, string name, string value)
        {
            AcfDataset acfDataset = Dataset.FirstOrDefault(x => x.PaymentDay == paymentDay);
            if (acfDataset == null)
            {
                acfDataset = new AcfDataset(paymentDay);
                Dataset.Add(acfDataset);
            }

            AcfAsset acfAsset = Asset.FirstOrDefault(x => x.Asset.Id == asset.Id);
            if (acfAsset == null)
            {
                acfAsset = new AcfAsset(asset);
                Asset.Add(acfAsset);
            }

            AcfUnit acfUnit = null;
            if (acfDataset.Exists(asset.Id))
            {
                acfUnit = acfDataset.Get(asset.Id);
                if (!acfAsset.Exists(paymentDay))
                {
                    acfAsset.Add(acfUnit);
                }
            }
            else if (acfAsset.Exists(paymentDay))
            {
                acfUnit = acfAsset.Get(paymentDay);
                if (!acfDataset.Exists(asset.Id))
                {
                    acfDataset.Add(acfUnit);
                }
            }
            else
            {
                acfUnit = new AcfUnit(asset, paymentDay);
                acfDataset.Add(acfUnit);
                acfAsset.Add(acfUnit);
            }

            acfUnit.SetValue(name, value);
        }

        public List<OverrideResultViewModel> MergeOsa(OverrideSingleAssetLogicModel osa)
        {
            var osaResults = new List<OverrideResultViewModel>();
            var acfDataset = this.Dataset.FirstOrDefault(x => x.PaymentDay == osa.PaymentDay);
            if (acfDataset == null)
            {
                return osaResults;
            }

            var oldSumDataset = acfDataset.Sum.DeepCopy();

            acfDataset.ForEach(unit =>
            {
                var assetName = unit.Asset.Name;
                var oldSumUnit = unit.Sum;

                var overridePrincipal = osa.GetPrincipal(unit.Asset.Id);
                if (overridePrincipal != null
                    && MathUtils.MoneyNE(unit.Principal, overridePrincipal.Principal))
                {
                    osaResults.Add(new OverrideResultViewModel(assetName, "本金",
                        unit.Principal, overridePrincipal.Principal, overridePrincipal.Comment,
                        overridePrincipal.CreateUserName, overridePrincipal.CreateTime));

                    unit.Principal = overridePrincipal.Principal;
                }

                var overrideInterest = osa.GetInterest(unit.Asset.Id);
                if (overrideInterest != null
                    && MathUtils.MoneyNE(unit.Interest, overrideInterest.Interest))
                {
                    osaResults.Add(new OverrideResultViewModel(assetName, "利息",
                        unit.Interest, overrideInterest.Interest, overrideInterest.Comment,
                        overrideInterest.CreateUserName, overrideInterest.CreateTime));

                    unit.Interest = overrideInterest.Interest;
                }

                var overridePrincipalBalance = osa.GetPrincipalBalance(unit.Asset.Id);
                if (overridePrincipalBalance != null
                    && MathUtils.MoneyNE(unit.Performing, overridePrincipalBalance.PrincipalBalance))
                {
                    osaResults.Add(new OverrideResultViewModel(assetName, "剩余本金",
                        unit.Performing, overridePrincipalBalance.PrincipalBalance, overridePrincipalBalance.Comment,
                        overridePrincipalBalance.CreateUserName, overridePrincipalBalance.CreateTime));

                    unit.Performing = overridePrincipalBalance.PrincipalBalance;
                }

                if (MathUtils.MoneyNE(oldSumUnit, unit.Sum))
                {
                    osaResults.Add(new OverrideResultViewModel(assetName, "合计", oldSumUnit, unit.Sum));
                }
            });

            ReCalcSum();

            if (MathUtils.MoneyNE(acfDataset.Sum.Interest, oldSumDataset.Interest))
            {
                osaResults.Add(new OverrideResultViewModel("总计", "利息", oldSumDataset.Interest, acfDataset.Sum.Interest));
            }

            if (MathUtils.MoneyNE(acfDataset.Sum.Principal, oldSumDataset.Principal))
            {
                osaResults.Add(new OverrideResultViewModel("总计", "本金", oldSumDataset.Principal, acfDataset.Sum.Principal));
            }

            if (MathUtils.MoneyNE(acfDataset.Sum.Performing, oldSumDataset.Performing))
            {
                osaResults.Add(new OverrideResultViewModel("总计", "剩余本金", oldSumDataset.Performing, acfDataset.Sum.Performing));
            }

            if (MathUtils.MoneyNE(acfDataset.Sum.Loss, oldSumDataset.Loss))
            {
                osaResults.Add(new OverrideResultViewModel("总计", "损失", oldSumDataset.Loss, acfDataset.Sum.Loss));
            }

            if (MathUtils.MoneyNE(acfDataset.Sum.Defaulted, oldSumDataset.Defaulted))
            {
                osaResults.Add(new OverrideResultViewModel("总计", "违约", oldSumDataset.Defaulted, acfDataset.Sum.Defaulted));
            }

            if (MathUtils.MoneyNE(acfDataset.Sum.Sum, oldSumDataset.Sum))
            {
                osaResults.Add(new OverrideResultViewModel("总计", "合计", oldSumDataset.Sum, acfDataset.Sum.Sum));
            }

            return osaResults;
        }

        public DataTable ExtractAssetCashflowDataTable(ABSDeal absDeal, DateTime paymentDay)
        {
            var dt = new DataTable();
            var determinationDates = ABSDealUtils.GetDeterminationDatesByPaymentDates(absDeal);
            var colHeader = new List<string> { "资产", "项目" };
            colHeader.AddRange(determinationDates.Keys.Select(x => determinationDates[x].ToString("yyyy-MM-dd")));
            colHeader.ForEach(x => dt.Columns.Add(x));

            var selectName = new List<string> { "利息", "本金", "剩余本金", "损失", "违约", "合计", "费用" };
            var selectValue = new List<Func<AcfUnitBase, double>> {
                x => x.Interest,
                x => x.Principal,
                x => x.Performing,
                x => x.Loss,
                x => x.Defaulted,
                x => x.Sum,
                x => x.Fee,
            };

            var dataResult = new List<List<string>>();
            foreach (var acfAsset in Asset)
            {
                for (int i = 0; i < selectName.Count; i++)
                {
                    var row = new List<string>();
                    row.Add(acfAsset.Asset.DisplayName);
                    row.Add(selectName[i]);

                    foreach (var acfUnit in acfAsset)
                    {
                        var value = selectValue[i](acfUnit);
                        row.Add(value.ToString("n2"));
                    }

                    var dtRow = dt.NewRow();
                    dtRow.ItemArray = row.ToArray();
                    dt.Rows.Add(dtRow);
                }
            }

            for (int i = 0; i < selectName.Count; i++)
            {
                var row = new List<string>();
                row.Add("总计");
                row.Add("总" + selectName[i]);
                foreach (var acfDataset in Dataset)
                {
                    var value = selectValue[i](acfDataset.Sum);
                    row.Add(value.ToString("n2"));
                }

                var dtRow = dt.NewRow();
                dtRow.ItemArray = row.ToArray();
                dt.Rows.Add(dtRow);
            }

            return dt;
        }

        /// <summary>
        /// 抽出资产端现金流表
        /// </summary>
        public object ExtractAssetCashflowTable(ABSDeal absDeal, DateTime paymentDay)
        {
            var determinationDates = ABSDealUtils.GetDeterminationDatesByPaymentDates(absDeal);
            var colHeader = new List<string> { "资产", "项目" };
            colHeader.AddRange(determinationDates.Keys.Select(x => determinationDates[x].ToString("yyyy-MM-dd")));

            var selectName = new List<string> { "利息", "本金", "剩余本金", "损失", "违约", "合计", "费用" };
            var selectValue = new List<Func<AcfUnitBase, double>> {
                x => x.Interest,
                x => x.Principal,
                x => x.Performing,
                x => x.Loss,
                x => x.Defaulted,
                x => x.Sum,
                x => x.Fee,
            };

            var dataResult = new List<List<string>>();
            foreach (var acfAsset in Asset)
            {
                for (int i = 0; i < selectName.Count; i++)
                {
                    var row = new List<string>();
                    row.Add(acfAsset.Asset.DisplayName);
                    row.Add(selectName[i]);

                    foreach (var acfUnit in acfAsset)
                    {
                        var value = selectValue[i](acfUnit);
                        row.Add(value.ToString("n2"));
                    }
                    dataResult.Add(row);
                }
            }

            for (int i = 0; i < selectName.Count; i++)
            {
                var row = new List<string>();
                row.Add("总计");
                row.Add("总" + selectName[i]);
                foreach (var acfDataset in Dataset)
                {
                    var value = selectValue[i](acfDataset.Sum);
                    row.Add(value.ToString("n2"));
                }

                dataResult.Add(row);
            }

            List<object> mergeCellsInfo = new List<object>();
            for (int i = 0; i < Asset.Count + 1; i++)
            {
                mergeCellsInfo.Add(new
                {
                    row = i * 7,
                    col = 0,
                    rowspan = 7,
                    colspan = 1,
                });
            };

            var result = new
            {
                colHeader = colHeader,
                paymentDate = paymentDay.ToString("yyyy-MM-dd"),
                dataResult = dataResult,
                mergeCellsInfo = mergeCellsInfo,
                isError = false
            };

            return result;
        }
 
        //抽出某期资产信息汇总为Table
        public DataTable ExtractCurrPeriodAssetSummaryTable(DateTime paymentDay)
        {
            var columnNames = new List<string>() { "公司名称", "利息", "本金", "剩余本金", "损失", "违约", "合计", "费用" };
            var dt = new DataTable();
            dt.Columns.AddRange(columnNames.Select(x => new DataColumn(x)).ToArray());

            foreach (var assetResult in Asset)
            {
                var assetUnit = assetResult.Get(paymentDay);
                var row = dt.NewRow();
                row.ItemArray = new object[] {
                    assetUnit.Asset.Name,
                    assetUnit.Interest.ToString("n2"),
                    assetUnit.Principal.ToString("n2"),
                    assetUnit.Performing.ToString("n2"),
                    assetUnit.Loss.ToString("n2"),
                    assetUnit.Defaulted.ToString("n2"),
                    assetUnit.Sum.ToString("n2"),
                    assetUnit.Fee.ToString("n2"),
                };
                dt.Rows.Add(row);
            }

            var datasetReuslt = Dataset.FirstOrDefault(x => x.PaymentDay == paymentDay);
            if (datasetReuslt != null)
            {
                var assetSum = datasetReuslt.Sum;
                var sumRow = dt.NewRow();
                sumRow.ItemArray = new object[] {
                    "总计",
                    assetSum.Interest.ToString("n2"),
                    assetSum.Principal.ToString("n2"),
                    assetSum.Performing.ToString("n2"),
                    assetSum.Loss.ToString("n2"),
                    assetSum.Defaulted.ToString("n2"),
                    assetSum.Sum.ToString("n2"),
                    assetSum.Fee.ToString("n2"),
                };
                dt.Rows.Add(sumRow);
            }
            return dt;
        }

        public List<AcfDataset> Dataset { get; set; }

        public List<AcfAsset> Asset { get; set; }
    }

    public struct OverrideResultViewModel
    {
        public OverrideResultViewModel(string assetName, string columnName, double originValue,
            double overrideValue, string comment = null, string createUserName = null, DateTime? createTime = null)
        {
            this.assetName = assetName;
            this.columnName = columnName;
            this.originValue = originValue.ToString("n2");
            this.overrideValue = overrideValue.ToString("n2");
            this.comment = comment ?? "-";
            this.createUserName = createUserName ?? "-";
            this.createTime = createTime.HasValue ? createTime.Value.ToString("yyyy-MM-dd") : "-";
        }

        public string assetName;
        public string columnName;
        public string originValue;
        public string overrideValue;
        public string comment;
        public string createTime;
        public string createUserName;
    }

    public class CurrPerdictPrincipalInterest
    {
        public CurrPerdictPrincipalInterest()
        {
            CurrPerdictInterest = 0.0;
            CurrPerdictPrincipal = 0.0;
        }
        public double CurrPerdictPrincipal { get; set; }

        public double CurrPerdictInterest { get; set; }
    }

}
