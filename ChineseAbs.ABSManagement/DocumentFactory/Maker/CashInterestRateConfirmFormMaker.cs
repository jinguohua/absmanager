using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Linq;

namespace ChineseAbs.ABSManagement.DocumentFactory.Maker
{
    public class CashInterestRateConfirmFormMaker : DocumentMakerBase
    {
        public CashInterestRateConfirmFormMaker(string userName)
            :base(userName)
        {
        }

        protected override string GetPatternFilePath()
        {
            return DocumentPattern.GetPath(m_project, DocPatternType.CashInterestRateConfirmForm);
        }

        private Tuple<Note, NoteData> GetNoteInfo()
        {
            var dataset = m_dbAdapter.Dataset.GetDatasetByDurationPeriod(m_project.ProjectId, m_paymentDay);
            var notes = m_dbAdapter.Dataset.GetNotes(m_project.ProjectId);
            var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(dataset.DatasetId);
            var cnabsNotes = new ProjectLogicModel(m_userName, m_project).Notes;

            Func<Note, bool> filterSecurityCode = (x) =>
                m_docName.Contains("(" + x.SecurityCode + ")")
                || m_docName.Contains("（" + x.SecurityCode + "）");

            Func<Note, bool> filterNoteShortName = (x) =>
                m_docName.Contains("(" + x.ShortName + ")")
                || m_docName.Contains("（" + x.ShortName + "）");

            var filters = new[] { filterSecurityCode, filterNoteShortName };

            foreach (var filter in filters)
            {
                if (cnabsNotes.Any(filter))
                {
                    var cnabsNote = cnabsNotes.First(filter);

                    CommUtils.Assert(notes.Any(x => x.NoteName == cnabsNote.NoteName),
                        "找不到证券[{0}]相关信息", cnabsNote.NoteName);
                    var note = notes.First(x => x.NoteName == cnabsNote.NoteName);
                    var noteData = noteDatas.Single(x => x.NoteId == note.NoteId);

                    CommUtils.Assert(noteData.PrincipalPaid.HasValue
                        && noteData.EndingBalance.HasValue
                        && noteData.InterestPaid.HasValue,
                        "兑付日为[{0}]的偿付期内，证券端现金流类型的工作未核对",
                        dataset.PaymentDate.Value.ToString("yyyy-MM-dd"));

                    return Tuple.Create(cnabsNote, noteData);
                }
            }

            return null;
        }

        protected override object MakeObjectInstance()
        {
            var noteInfo = GetNoteInfo();
            CommUtils.AssertNotNull(noteInfo, "无法通过文档名称[" + m_docName + "]获取证券(noteInfo)信息<br/>文档名格式：兑付兑息确认表-证券名（证券代码）.docx");
            var note = noteInfo.Item1;
            var noteData = noteInfo.Item2;
            CommUtils.AssertNotNull(note, "无法通过文档名称[" + m_docName + "]获取证券(note)信息<br/>文档名格式：兑付兑息确认表-证券名（证券代码）.docx");
            CommUtils.AssertNotNull(noteData, "无法通过文档名称[" + m_docName + "]获取证券(noteData)信息<br/>文档名格式：兑付兑息确认表-证券名（证券代码）.docx");

            var obj = new CashInterestRateConfirmForm();

            //证券代码
            obj.BondCode = note.SecurityCode;

            //证券简称
            obj.ShortBond = note.NoteName;

            //多种命名方式
            obj.NameCN = note.NoteName;
            obj.NameCNHyphen = GenerateNameCNHyphen(note);
            obj.NameCNFullHyphen = obj.NameCNHyphen + "级资产支持证券";
            obj.NameEN = note.ShortName;
            obj.NameENHyphen = InsertHyphenBeforeNumber(obj.NameEN);
            obj.NameENUnderline = obj.NameENHyphen.Replace("-", "_");

            //是否浮动利率
            obj.IsFloatInterestRate = "否";
            
            //是否分期偿还 = （当期兑付本金!=0 && 剩余本金!=0 ）? 是 : 否   ----From WJGY
            var isAmortize = noteData.EndingBalance.Value != 0 && noteData.PrincipalPaid.Value != 0;

            //是否分期偿还
            obj.IsAmortize = isAmortize ? "是" : "否";

            //份数 = 本金 / 100
            var notional100 = note.Notional.Value / 100;

            //每百元兑付（兑息）金额 = 兑付利息金额 / 份数
            obj.CenturyInterestRateMoney = noteData.InterestPaid.Value / notional100;

            //每千元兑付（兑息）金额 = 每百元兑付（兑息）金额 * 10
            obj.ThousandInterestRateMoney = obj.CenturyInterestRateMoney * 10;

            //每百元分期偿还本金金额 = 是否分期偿还 ? ( 兑付本金金额 / 份数 ) : "-"
            obj.CenturyAmortizeMoney = noteData.PrincipalPaid.Value / notional100;

            //每千元分期偿还本金金额 = 是否分期偿还 ? 每百元分期偿还本金金额 * 10 : "-"
            obj.ThousandAmortizeMoney = obj.CenturyAmortizeMoney.Value * 10;

            if (!isAmortize)
            {
                obj.CenturyAmortizeMoney = null;
                obj.ThousandAmortizeMoney = null;
            }

            //代发证券数量 = 每档证券的本金
            obj.IssuingBondNum = note.Notional.Value;

            //兑付（兑息、分期偿还）金额 = 本期兑付的证券的本金 + 利息
            obj.InterestRateMoney = noteData.PrincipalPaid.Value + noteData.InterestPaid.Value;

            //手续费金额 = 本期兑付的证券的本金 + 利息的0.005%
            obj.FactorageMoney = obj.InterestRateMoney * 0.00005m;

            //合计金额 = 兑付（兑息、分期偿还）金额 + 手续费金额
            obj.TotalMoney = obj.InterestRateMoney + obj.FactorageMoney;

            var logicModel = new ProjectLogicModel(m_userName, m_project);
            var dataset = logicModel.DealSchedule.GetByPaymentDay(m_paymentDay).Dataset;
            var paymentDate = dataset.DatasetSchedule.PaymentDate;
            CommUtils.Assert(dataset.HasDealModel, "找不到模型：产品={0}；偿付期={1}", m_project.Name, paymentDate);

            //兑付兑息日使用 兑付日
            obj.InterestRateDay = paymentDate;

            //证券登记日使用 工作 的截止日的签前一个工作日
            var t_1 = DateUtils.AddTradingDay(paymentDate, -1);
            var t_3 = DateUtils.AddTradingDay(paymentDate, -3);
            obj.DebtRegisterDay = MathUtils.MoneyEQ((double)noteData.EndingBalance.Value, 0) ? t_3 : t_1;

            //备注不填
            obj.Remark = string.Empty;

            //申请日 使用当天
            obj.ApplicationDate = DateTime.Now; 

            return obj;
        }
    }
}
