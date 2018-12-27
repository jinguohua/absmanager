using ChineseAbs.ABSManagement.Models;
using System.Linq;
using ABSMgrConn;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Manager
{
    public class DMSDurationManagementPlatformManager
        : BaseModelManager<DMSDurationManagementPlatform, ABSMgrConn.TableDMSDurationManagementPlatform>
    {
        public DMSDurationManagementPlatformManager()
        {
            m_defaultTableName = "dbo.DMSDurationManagementPlatform";
            m_defaultPrimaryKey = "dms_duration_management_platform_id";
            m_defalutFieldPrefix = "dms_duration_management_platform_";
        }

        public List<DMSDurationManagementPlatform> GetByProjectId(string projectId)
        {
            var DmsDurations= Select<TableDMSDurationManagementPlatform>("project_id", projectId);
            return DmsDurations.Select(x => new DMSDurationManagementPlatform(x)).ToList();
        }


    }
}
