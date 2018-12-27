using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Linq;

namespace ChineseAbs.ABSManagement.DocumentFactory.Maker
{
    public class InterestPaymentPlanApplicationMaker : DocumentMakerBase
    {
        public InterestPaymentPlanApplicationMaker(string userName)
            :base(userName)
        {
        }

        protected override string GetPatternFilePath()
        {
            return DocumentPattern.GetPath(m_project, DocPatternType.InterestPaymentPlanApplication);
        }

        protected override object MakeObjectInstance()
        {
            var obj = new InterestPaymentPlanApplication();

            if (m_docName.StartsWith("付息方案"))
            {
                CommUtils.Assert(m_docName.Contains("级"), "文档名不包含[级]字，付息方案文档名格式 [付息方案XX级-XXXXXX-XX.XX].");
                var text = m_docName.Substring(4);
                obj.BondShortName = text.Substring(0, text.IndexOf("级"));

                text = text.Replace(obj.BondShortName, "");
                text = text.Substring(2);

                var indexOfUnderlineBeforeDate = text.IndexOf("-");
                if (indexOfUnderlineBeforeDate != -1)
                {
                    obj.BondCode = text.Substring(0, indexOfUnderlineBeforeDate);

                    text = text.Replace(obj.BondCode, "");
                    text = text.Substring(1);
                    var subText = text.Split(new char[] { '.' });
                    obj.Date = new DateTime(m_timeStamp.Year, int.Parse(subText[0]), int.Parse(subText[1]));
                }
                else
                {
                    obj.BondCode = text;
                    obj.Date = m_timeStamp;
                }
            }
            else
            {
                CommUtils.Assert(m_docName.Contains("付息方案"), "文档名必须包含[付息方案]"
                    + "<br/>例如：付息方案A1级-142815-03.04.");

                var firstIndexOfUnderline = m_docName.IndexOf("-");
                CommUtils.Assert(firstIndexOfUnderline != -1, "文档名称格式错误"
                    + "<br/>例如：付息方案A1级-142815-03.04");

                var text = m_docName.Substring(firstIndexOfUnderline + 1);
                var secondIndexOfUnderline = text.IndexOf("-");
                CommUtils.Assert(secondIndexOfUnderline != -1, "文档名称格式错误"
                    + "<br/>例如：付息方案A1级-142815-03.04");

                obj.BondShortName = text.Substring(0, secondIndexOfUnderline).Replace("级", "");
                obj.BondCode = text.Substring(secondIndexOfUnderline + 1);

                var indexOfUnderlineBeforeDate = obj.BondCode.IndexOf("-");
                if (indexOfUnderlineBeforeDate != -1)
                {
                    var dateText = obj.BondCode.Substring(indexOfUnderlineBeforeDate + 1);
                    var subText = dateText.Split(new char[] { '.' });
                    obj.Date = new DateTime(m_timeStamp.Year, int.Parse(subText[0]), int.Parse(subText[1]));
                }
                else
                {
                    obj.Date = m_timeStamp;
                }
            }

            if (obj.BondCode.EndsWith(".docx", StringComparison.CurrentCultureIgnoreCase))
            {
                obj.BondCode = obj.BondCode.Substring(0, obj.BondCode.Length - 5);
            }


            obj.RegisterDate = DateUtils.GetNextTradingDay(m_timeStamp);
            obj.FileDate = DateUtils.GetNextTradingDay(obj.RegisterDate);

            var projectLogicModel = new ProjectLogicModel(m_userName, m_project.ProjectId);
            var dataset = projectLogicModel.DealSchedule.GetByPaymentDay(m_paymentDay).Dataset.Instance;
            var notes = m_dbAdapter.Dataset.GetNotes(m_project.ProjectId);
            var noteDatas = m_dbAdapter.Dataset.GetNoteDatas(dataset.DatasetId);
            var cnabsNotes = new ProjectLogicModel(m_userName, m_project).Notes;
            var cnabsNote = cnabsNotes.Single(x => x.SecurityCode == obj.BondCode);

            CommUtils.Assert(notes.Any(x => x.NoteName == cnabsNote.NoteName),
                "找不到证券[{0}]相关信息", cnabsNote.NoteName);

            var note = notes.First(x => x.NoteName == cnabsNote.NoteName);
            var noteData = noteDatas.Single(x => x.NoteId == note.NoteId);
            CommUtils.Assert(noteData.InterestPaid.HasValue, "对应的证券端现金流工作未核对");
            obj.Money = noteData.InterestPaid.Value;
            return obj;
        }
    }
}
