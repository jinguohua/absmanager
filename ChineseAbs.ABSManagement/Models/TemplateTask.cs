using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Utils;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class TemplateTask : BaseDataContainer<TableTemplateTask>
    {
        public TemplateTask()
        {
        }

        public TemplateTask(ABSMgrConn.TableTemplateTask templateTask)
            : base(templateTask)
        {
        }

        public int TemplateTaskId { get; set; }
        
        public string TemplateTaskGuid { get; set; }
        
        public int? TemplateId { get; set; }
        
        public string TemplateTaskName { get; set; }

        public string BeginDate { get; set; }

        public string TriggerDate { get; set; }

        public List<int> PrevIds { get; set; }

        public string TemplateTaskDetail { get; set; }

        public string TemplateTaskTarget { get; set; }

        public string TemplateTaskExtensionName { get; set; }

        public override TableTemplateTask GetTableObject()
        {
            TableTemplateTask table = new TableTemplateTask();
            table.template_task_id = TemplateTaskId;
            table.template_task_guid = TemplateTaskGuid;
            table.template_id = TemplateId;
            table.template_task_name = TemplateTaskName;
            table.begin_date = BeginDate;
            table.trigger_date = TriggerDate;
            table.prev_template_task_ids = CommUtils.Join(PrevIds.ConvertAll(x => x.ToString()));
            table.template_task_detail = TemplateTaskDetail;
            table.template_task_target = TemplateTaskTarget;
            table.template_task_extension_name = TemplateTaskExtensionName;
            return table;
        }

        public override void FromTableObject(TableTemplateTask templateTask)
        {
            TemplateTaskId = templateTask.template_task_id;
            TemplateTaskGuid = templateTask.template_task_guid;
            TemplateId = templateTask.template_id;
            TemplateTaskName = templateTask.template_task_name;
            BeginDate = templateTask.begin_date;
            TriggerDate = templateTask.trigger_date;
            PrevIds = CommUtils.Split(templateTask.prev_template_task_ids).ToList()
                .ConvertAll(item => int.Parse(item.Trim()));
            TemplateTaskDetail = templateTask.template_task_detail;
            TemplateTaskTarget = templateTask.template_task_target;
            TemplateTaskExtensionName = templateTask.template_task_extension_name;
        }
    }
}
