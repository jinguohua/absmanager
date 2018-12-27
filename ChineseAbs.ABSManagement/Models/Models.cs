using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Models
{
    public class Models: List<Model>
    {
        public List<ABSMgrConn.TableModels> ToTableList()
        {
            var tableList = new List<ABSMgrConn.TableModels>();
            foreach (var item in this)
            {
                tableList.Add(item.GetTableObject());
            }
            return tableList;
        }
    }

    public class Model : BaseDataContainer<ABSMgrConn.TableModels>
    {
        public Model(ABSMgrConn.TableModels obj) : base(obj) { }

        public Model() { }

        public int ModelId { get; set; }

        public string ModelName { get; set; }

        public string ModelFolder { get; set; }

        public DateTime TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }

        public string ModelJson { get; set; }

        public override ABSMgrConn.TableModels GetTableObject()
        {
            var t = new ABSMgrConn.TableModels();
            t.model_id = ModelId;
            t.model_name = ModelName;
            t.model_folder = ModelFolder;
            t.time_stamp = TimeStamp;
            t.time_stamp_user_name = TimeStampUserName;
            t.model_json = ModelJson;
            return t;
        }

        public override void FromTableObject(ABSMgrConn.TableModels obj)
        {
            this.ModelId = obj.model_id;
            this.ModelName = obj.model_name;
            this.ModelFolder = obj.model_folder;
            this.TimeStamp = obj.time_stamp;
            this.TimeStampUserName = obj.time_stamp_user_name;
        }
    }
}
