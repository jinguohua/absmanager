using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Objects
{
    public class ColumnConfigItem
    {
        public string Name { get; set; }

        public string Field { get; set; }

        public string SystemField { get; set; }

        public string Type { get; set; }

        public EColumnDateType EType
        {
            get
            {
                EColumnDateType type = EColumnDateType.Text;
                if (!String.IsNullOrEmpty(Type))
                {
                    if (Enum.TryParse<EColumnDateType>(Type, true, out type))
                        return type;
                }
                return EColumnDateType.Text;
            }
        }
    }

    public class ColumnsConfig
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public int Id { get; set; }

        public List<ColumnConfigItem> Items { get; set; }
    }

    public enum EColumnDateType
    {
        [Description("字符串")]
        Text,

        [Description("日期")]
        Date,

        [Description("数字")]
        Numeric
    }
}
