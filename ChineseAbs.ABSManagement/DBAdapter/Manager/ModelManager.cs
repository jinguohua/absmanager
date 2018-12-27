using ABSMgrConn;
using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public class ModelManager : BaseManager
    {
        public ModelManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public int GetModelID(string modelName)
        {
            var ids = m_db.Query<int>(
                "SELECT DISTINCT(model_id) FROM dbo.Models WHERE model_name=@0", modelName);

            if (ids.Count() != 1)
            {
                throw new ApplicationException("Get Model [" + modelName + "] error.");
            }

            return ids.Single();
        }

        public IEnumerable<TableModels> GetModels()
        {
            return m_db.Query<TableModels>("SELECT * FROM dbo.Models" + m_orderBy);
        }

        public int UpdateModel(Model model)
        {
            var modelTable = model.GetTableObject();
            return m_db.Update("Models", "model_id", modelTable, model.ModelId);
        }

        public int NewModel(string modelName)
        {
            ABSMgrConn.TableModels model = new ABSMgrConn.TableModels();
            model.model_name = modelName;
            model.model_folder = string.Empty;
            model.dataset = string.Empty;
            model.time_stamp = DateTime.Now;
            model.time_stamp_user_name = UserInfo.UserName;
            var modelId = (int)m_db.Insert("Models", "model_id", true, model);

            return modelId;
        }

        public int NewModel(Model model)
        {
            var modelTable = model.GetTableObject();
            return (int)m_db.Insert("Models", "model_id", true, model);
        }

        public bool SaveSimpleModel(Model model)
        {
            Model mj = m_db.SingleOrDefault<Model>("SELECT * FROM dbo.Models WHERE model_id=@0", model.ModelId);
            var modelTable = model.GetTableObject();
            if (mj == null)
            {
                m_db.Insert("Models", "model_id", true, modelTable);
                return modelTable.model_id > 0;
            }
            else
            {
                return m_db.Update(modelTable) > 0;
            }
        }

        /// <summary>
        /// 从ChineseABS中获取所有的Deal
        /// </summary>
        public List<Tuple<int, string>> GetAllDeals()
        {
            var records = m_db.Query<ABSMgrConn.VwDeal>("SELECT * FROM dbo.vw_deal ORDER BY deal_id");
            return records.ToList().ConvertAll(x => new Tuple<int, string>(x.deal_id, x.deal_name));
        }

        /// <summary>
        /// 从ChineseABS中获取指定Deal的note信息
        /// </summary>
        public List<Note> GetNoteByDealId(int dealId)
        {
            var records = m_db.Query<ABSMgrConn.VwNote>("SELECT * FROM " + m_db.ChineseABSDB + "dbo.Note WHERE deal_id = @0 ORDER BY note_id", dealId);

            List<Note> notes = new List<Note>();
            foreach (var record in records)
            {
                var note = new Note();
                note.NoteId = record.note_id;
                note.ProjectId = -1;
                note.ShortName = record.name;
                note.NoteName = record.description;
                note.Notional = record.notional.HasValue ? (decimal)record.notional.Value : 0m;
                note.IsEquity = record.is_equity.HasValue ? record.is_equity.Value : false;

                note.SecurityCode = record.security_code;
                note.CouponString = record.coupon_string;
                note.ExpectedMaturityDate = record.expected_maturity_date;
                note.AccrualMethod = record.accrual_method;

                notes.Add(note);
            }
            return notes;
        }

        /// <summary>
        /// 从ChineseABS中获取指定Deal的paymentDate信息
        /// </summary>
        public List<DateTime> GetPaymentDates(int dealId)
        {
            var items = m_db.Query<ABSMgrConn.VwPaymentDates>(
                "SELECT * FROM "+ m_db.ChineseABSDB +"[dbo].[PaymentDates] WHERE deal_id = @0 ORDER BY payment_date",
                dealId);

            return items.ToList().Where(x => x.payment_date.HasValue).ToList().ConvertAll(x => x.payment_date.Value);
        }

        private const string m_orderBy = " ORDER BY dbo.Models.model_id DESC";
    }
}
