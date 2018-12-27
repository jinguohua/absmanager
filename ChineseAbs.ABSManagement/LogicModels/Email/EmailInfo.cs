using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.LogicModels.Email
{
    public class EmailInfos
    {
        public int Email_Context_Id { get; set; }
        public string email_context_guid { get; set; }
        public string content { get; set; }
        public DateTime create_time { get; set; }
        public string create_user_name { get; set; }
        public DateTime last_modify_time { get; set; }
        public string last_modify_user_name { get; set; }
        public int record_status_id { get; set; }
    }
}
