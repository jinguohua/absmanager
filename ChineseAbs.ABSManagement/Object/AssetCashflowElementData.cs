using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Object
{
    public class AssetCashflowElementData
    {
        public AssetCashflowElementData()
        {
            Values = new List<DateValuePair>();
        }
        public string AssetId { get; set; }

        public string Description { get; set; }

        public List<DateValuePair> Values { get; set; }

    }

    public class DateValuePair
    {
        public DateTime PaymentDate { get; set; }

        public DateTime DeterminationDate { get; set; }

        public string Value { get; set; }
    }

    public class AssetCashflowData
    {
        public AssetCashflowData()
        {
            Values = new List<AssetCashflowDateValuePair>();
        }

        public void AddValue(string name, string value)
        {
            Values.Add(new AssetCashflowDateValuePair(name, value));
        }

        public List<AssetCashflowDateValuePair> Values { get; set; }

        public string RowName { get; set; }
    }

    public class AssetCashflowDateValuePair
    {
        public AssetCashflowDateValuePair()
        {

        }

        public AssetCashflowDateValuePair(string name, string value)
        {
            ColumnName = name;
            Value = value;
        }

        public string ColumnName { get; set; }

        public string Value { get; set; }
    }
}
