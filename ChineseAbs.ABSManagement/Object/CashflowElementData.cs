using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Object
{
    public class CashflowElementData
    {
        public CashflowElementData()
        {
            Values = new List<DateValuePair>();
        }

        public List<DateValuePair> Values { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }

    public class CashflowVariablesData
    {
        public string CnName { get; set; }

        public string EnName { get; set; }

        public double Value { get; set; }
    }

    public class CashflowData
    {
        public CashflowData()
        {
            Values = new List<CashflowDateValuePair>();
        }
        public List<CashflowDateValuePair> Values { get; set; }

        public string RowName { get; set; }
    }

    public class CashflowDateValuePair
    {
        public string ColumnName { get; set; }

        public string Value { get; set; }
    }
}
