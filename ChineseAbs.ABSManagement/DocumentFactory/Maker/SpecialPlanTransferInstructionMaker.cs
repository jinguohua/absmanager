using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.CalcService.Data.NancyData;
using System;
using System.Data;
using System.Linq;

namespace ChineseAbs.ABSManagement.DocumentFactory.Maker
{
    public class SpecialPlanTransferInstructionMaker : DocumentMakerBase
    {
        public SpecialPlanTransferInstructionMaker(string userName)
            :base(userName)
        {
        }

        protected override string GetPatternFilePath()
        {
            return DocumentPattern.GetPath(m_project, DocPatternType.SpecialPlanTransferInstruction);
        }

        private decimal GetFee(string feeName)
        {
            //var dsVariables = NancyUtils.GetOverridableVariables(m_project.ProjectId);

            var logicModel = new ProjectLogicModel(m_userName, m_project.ProjectId);
            var dataset = logicModel.DealSchedule.GetByPaymentDay(m_paymentDay).Dataset;
            NancyStaticAnalysisResult results = NancyUtils.GetStaticAnalyticsResult(m_project.ProjectId,
                null, dataset.Instance.AsOfDate, dataset.AssetOverrideSetting);

            var cfTable = results.CashflowDt;

            int columnIndex = -1;
            for (int i = 0; i < cfTable.Columns.Count; ++i)
            {
                DateTime columnDate;
                if (DateTime.TryParse(cfTable.Columns[i].ColumnName, out columnDate)
                    && m_paymentDay == columnDate)
                {
                    columnIndex = i;
                    break;
                }
            }

            CommUtils.Assert(columnIndex >= 0, "找不到对应支付日[" + m_paymentDay.ToString() + "]的数据");

            for (int i = 0; i < cfTable.Rows.Count; ++i)
            {
                var row = cfTable.Rows[i];
                if (row.ItemArray[1].Equals(feeName))
                {
                    return decimal.Parse(row.ItemArray[columnIndex].ToString());
                }
            }

            throw new ApplicationException("Can't find [" + feeName + "].");
        }

        bool MatchDocName(string keyword)
        {
            return m_docName.Contains("(" + keyword + ")")
                || m_docName.Contains("（" + keyword + "）")
                || m_docName.EndsWith("-" + keyword);
        }

        private void GetMoney(SpecialPlanTransferInstruction obj, DataTable cashflowDt)
        {
            if (MatchDocName("托管费"))
            {
                m_usesAndComment = "支付托管费";
                obj.Money = GetFee("TrusteeFee.Received");
            }
            else if (MatchDocName("资产服务机构费用"))
            {
                obj.Money = GetFee("ServiceFee.Received");
            }
            else if (MatchDocName("次级收益"))
            {
                m_usesAndComment = "支付次级收益";
                obj.Money = GetFee("Sub.Interest Received");
            }
            else
            {
                var noteInfo = GetNoteInfo();
                CommUtils.AssertNotNull(noteInfo, "无法通过[" + m_docName + "]获取相关note信息。");
                var noteData = noteInfo.NoteData;
                CommUtils.AssertNotNull(noteData, "无法通过[" + m_docName + "]获取相关note data信息。");

                //Money: 金额 = 本金 + 利息
                obj.Money = noteData.PrincipalPaid.Value + noteData.InterestPaid.Value;

                //MoneyWithCsdcFee： 金额 = 本金 + 利息 + 中证手续费
                //中证手续费 = (本金 + 利息) * 0.005%
                //金额 = (本金 + 利息) * 1.00005

                var csdcFee = 0.0m;
                var csdcFeeRowIndex = cashflowDt.IndexOfRow(x => x.ItemArray[1].ToString() == noteInfo.Note.ShortName + ".CsdcFee");
                if (csdcFeeRowIndex != -1)
                {
                    for (int i = 0; i < cashflowDt.Columns.Count; i++)
                    {
                        var column = cashflowDt.Columns[i];
                        DateTime columnDate;
                        if (DateTime.TryParse(column.ToString(), out columnDate)
                            && columnDate == m_paymentDay)
                        {
                            decimal.TryParse(cashflowDt.Rows[csdcFeeRowIndex][i].ToString(), out csdcFee);
                            break;
                        }
                    }
                }

                obj.MoneyWithCsdcFee = obj.Money + csdcFee;

                //MoneyWithCsdcFee1000： 金额 = 本金 + 利息 + 中证手续费 + 1000元长款逻辑
                // 小数点第三位后有值（比如56.321）：金额 + 1000
                // 小数点第三位后没有有值（比如56.32）：不处理
                CommUtils.AssertNotNull(noteInfo.Note.Notional, "证券[{0}]初始本金不应为0", noteInfo.Note.NoteName);
                CommUtils.Assert(noteInfo.Note.Notional.Value != 0m, "证券[{0}]初始本金不应为0", noteInfo.Note.NoteName);
                var notional = noteInfo.Note.Notional.Value;
                
                //每百元偿付金额
                var moneyPer100 = obj.MoneyWithCsdcFee / notional * 100;
                var add = (decimal)((int)(moneyPer100 * 100)) != moneyPer100 * 100;
                obj.MoneyWithCsdcFee1000 = add ? (obj.MoneyWithCsdcFee + 1000) : obj.MoneyWithCsdcFee;
            }

            obj.MoneyCN = obj.Money.ToCnString();
            obj.MoneyWithCsdcFeeCN = obj.MoneyWithCsdcFee.ToCnString();
            obj.MoneyWithCsdcFee1000CN = obj.MoneyWithCsdcFee1000.ToCnString();
        }

        private class NoteInfo
        {
            public NoteData NoteData { get; set; }
            public Note Note { get; set; }
        }

        private NoteInfo GetNoteInfo()
        {
            var datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(m_project.ProjectId);
            var findDatasets = datasets.Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value == m_paymentDay).ToList();
            findDatasets.Sort((l, r) => l.AsOfDate.CompareTo(r.AsOfDate));
            CommUtils.Assert(findDatasets.Count >= 1, "Can' find any dataset of [" + m_paymentDay.ToString() + "].");
            var dataset = findDatasets[0];
            var notes = m_dbAdapter.Dataset.GetNotes(m_project.ProjectId);
            var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(dataset.DatasetId);
            var cnabsNotes = new ProjectLogicModel(m_userName, m_project).Notes;

            Func<Note, bool> filterSecurityCode = (x) =>
                m_docName.Contains("(" + x.SecurityCode + ")")
                || m_docName.Contains("（" + x.SecurityCode + "）");

            Func<Note, bool> filterNoteShortName = (x) =>
                m_docName.Contains("(" + x.ShortName + ")")
                || m_docName.Contains("（" + x.ShortName + "）");

            var filters = new [] { filterSecurityCode, filterNoteShortName };

            foreach (var filter in filters)
            {
                if (cnabsNotes.Any(filter))
                {
                    var cnabsNote = cnabsNotes.First(filter);

                    CommUtils.Assert(notes.Any(x => x.NoteName == cnabsNote.NoteName),
                        "找不到证券[{0}]相关信息", cnabsNote.NoteName);

                    var note = notes.First(x => x.NoteName == cnabsNote.NoteName);
                    m_usesAndComment = cnabsNote.SecurityCode + "兑付兑息款";
                    var noteData = noteDatas.Single(x => x.NoteId == note.NoteId);
                    return new NoteInfo
                    {
                        Note = note,
                        NoteData = noteData,
                    };
                }
            }

            return null;
        }

        private EAccountType GetBankType()
        {
            if (MatchDocName("托管费"))
            {
                return EAccountType.托管机构账户;
            }
            else if (MatchDocName("资产服务机构费用"))
            {
                return EAccountType.原始权益人账户;
            }
            else if (MatchDocName("次级收益"))
            {
                return EAccountType.原始权益人账户;
            }
            else
            {
                return EAccountType.登记机构账户;
            }
        }

        protected override object MakeObjectInstance()
        {
            var logicModel = new ProjectLogicModel(m_userName, m_project);
            var cashflowDt = logicModel.DealSchedule.GetByPaymentDay(m_paymentDay).Dataset.DealModel.CashflowDt;

            var obj = new SpecialPlanTransferInstruction();
            GetMoney(obj, cashflowDt);

            var allBankAccounts = m_dbAdapter.BankAccount.GetAccounts(m_project.ProjectId, true);

            //收款户
            var bankType = GetBankType();
            var payeeBanks = allBankAccounts.Where(x => x.AccountType == bankType).ToList();
            CommUtils.Assert(payeeBanks.Count < 2, "产品 [" + m_project.Name + "] 的[" + bankType.ToString() + "]不唯一，无法确定收款户。");

            var noPayeeBankMsg = "【缺少" + bankType.ToString() + "信息】";

            if (payeeBanks.Count == 1)
            {
                var payee = payeeBanks[0];
                obj.Payee.Name = payee.Name;
                obj.Payee.Bank = payee.IssuerBank;
                obj.Payee.Account = payee.BankAccount;
            }
            else
            {
                obj.Payee.Name = noPayeeBankMsg;
                obj.Payee.Bank = noPayeeBankMsg;
                obj.Payee.Account = noPayeeBankMsg;
            }

            //付款户
            var payerBanks = allBankAccounts.Where(x => x.AccountType == EAccountType.专项计划账户).ToList();
            CommUtils.Assert(payerBanks.Count < 2, "产品 [" + m_project.Name + "] 的[专项计划账户]不唯一，无法确定付款户。");

            var noPayerBankMsg = "【缺少" + EAccountType.专项计划账户.ToString() + "信息】";

            if (payerBanks.Count == 1)
            {
                var payer = payerBanks[0];
                obj.Payer.Name = payer.Name;
                obj.Payer.Bank = payer.IssuerBank;
                obj.Payer.Account = payer.BankAccount;
            }
            else
            {
                obj.Payer.Name = noPayerBankMsg;
                obj.Payer.Bank = noPayerBankMsg;
                obj.Payer.Account = noPayerBankMsg;
            }

            //划付日期 = 当前工作截止日期的下一个工作日
            obj.TransferDate = DateUtils.GetNextTradingDay(m_paymentDay);

            obj.UsesAndComment = m_usesAndComment;

            return obj;
        }

        private string m_usesAndComment;
    }
}
