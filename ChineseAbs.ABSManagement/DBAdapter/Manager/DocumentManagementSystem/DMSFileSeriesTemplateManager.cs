using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using ChineseAbs.ABSManagement.Utils;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.DocumentManagementSystem
{
    public class DMSFileSeriesTemplateManager
        : BaseModelManager<DMSFileSeriesTemplate, ABSMgrConn.TableDMSFileSeriesTemplate>
    {
        public DMSFileSeriesTemplateManager()
        {
            m_defaultTableName = "dbo.DMSFileSeriesTemplate";
            m_defaultPrimaryKey = "dms_file_series_template_id";
            m_defalutFieldPrefix = "dms_file_series_template_";
        }

        public DMSFileSeriesTemplate GetByFileSeriesId(int fileSeriesId)
        {
            var results = Select<ABSMgrConn.TableDMSFileSeriesTemplate>(m_defaultTableName, "dms_file_series_id", fileSeriesId)
                .ToList().ConvertAll(x => new DMSFileSeriesTemplate(x));
            if (results.Count == 0)
            {
                return null;
            }
            
            CommUtils.Assert(results.Count < 2, "找到了多条DMSFileSeriesTemplate记录：fileSeriesId=[{0}]", fileSeriesId);
            return results.Single();
        }
    }
}
