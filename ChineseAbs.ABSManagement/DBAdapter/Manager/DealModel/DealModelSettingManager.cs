
using ChineseAbs.ABSManagement.Models;
using System.Linq;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Manager
{
    public class DealModelSettingManager
        : BaseModelManager<DealModelSetting, ABSMgrConn.TableDealModelSetting>
    {
        public DealModelSettingManager()
        {
            m_defaultTableName = "dbo.DealModelSetting";
            m_defaultPrimaryKey = "deal_model_setting_id";
            m_defalutFieldPrefix = "deal_model_setting_";
        }

        public DealModelSetting GetByProjectId(int projectId)
        {
            var records = Select<ABSMgrConn.TableDealModelSetting>("project_id", projectId);
            var result = records.Select(x => new DealModelSetting(x)).FirstOrDefault();
            if (result == null)
            {
                result = new DealModelSetting()
                {
                    ProjectId = projectId,

                    //默认情况下，是预测模式
                    EnablePredictMode = true
                };
                result = New(result);
            }

            return result;
        }
    }
}
