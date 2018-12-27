
using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ChineseAbs.ABSManagement.Manager
{
    public class EditModelCSVManager
        : BaseModelManager<EditModelCSV, ABSMgrConn.TableEditModelCSV>
    {
        public EditModelCSVManager()
        {
            m_defaultTableName = "dbo.EditModelCSV";
            m_defaultPrimaryKey = "edit_model_csv_id";
            m_defalutFieldPrefix = "edit_model_csv_";
        }

        public List<EditModelCSV> GetByCsvType(EnumCsvType csvType,string projectGuid,string asOfDate)
        {
            var querys = Select<TableEditModelCSV>("type", csvType.ToString()).Where(x=>x.project_guid==projectGuid && x.asofdate==asOfDate).OrderByDescending(x => x.create_time);
            return querys.ToList().ConvertAll(x => new EditModelCSV(x));
        }

        public List<EditModelCSV> GetAll()
        {
            var querys = Select<TableEditModelCSV>("1", 1);
            return querys.ToList().ConvertAll(x => new EditModelCSV(x));
        }

        public EditModelCSV GetPrevCsvGuid(string guid, EnumCsvType enumCsvType, string projectGuid, string asOfDate)
        {
            var sameTypeList = GetByCsvType(enumCsvType,  projectGuid,  asOfDate);

            var createTime = sameTypeList.Where(x => x.Guid == guid).FirstOrDefault().CreateTime;
            var prevModelCsv = sameTypeList.Where(x => x.CreateTime < createTime).OrderByDescending(x => x.CreateTime).FirstOrDefault();
            if (prevModelCsv != null)
            {
                return prevModelCsv;
            };
            return null;
        }
    }

    public enum EnumCsvType
    {
        CurrentVariables,
        FutureVariables,
        PastVariables,
        collateral,
        AmortizationSchedule,
        PromisedCashflow,
    }
}
