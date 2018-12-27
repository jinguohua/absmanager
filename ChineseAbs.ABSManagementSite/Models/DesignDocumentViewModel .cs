using ChineseAbs.ABSManagement.Utils.TreeUtils;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class DesignDocumentViewModel
    {
        public DesignDocumentViewModel()
        {
            Tree = new Tree<PropInfo>();
        }

        public Tree<PropInfo> Tree { get; set; }
    }

    public class PropInfo
    {
        public SimpleDataType DataType { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public void SetDataType(string typeName)
        {
            DataType = m_dataTypeDict.ContainsKey(typeName) ? m_dataTypeDict[typeName] : SimpleDataType.Undefined;
        }

        static private readonly Dictionary<string, SimpleDataType> m_dataTypeDict = new Dictionary<string, SimpleDataType>
        {
            {"Int32", SimpleDataType.Int},
            {"Int", SimpleDataType.Int},
            {"int32", SimpleDataType.Int},
            {"int", SimpleDataType.Int},
            {"decimal", SimpleDataType.Decimal},
            {"Decimal", SimpleDataType.Decimal},
            {"double", SimpleDataType.Decimal},
            {"Double", SimpleDataType.Decimal},
            {"string", SimpleDataType.String},
            {"String", SimpleDataType.String},
            {"boolean", SimpleDataType.Bool},
            {"Boolean", SimpleDataType.Bool},
            {"bool", SimpleDataType.Bool},
            {"Bool", SimpleDataType.Bool},
            {"datetime",SimpleDataType.Decimal},
            {"DateTime",SimpleDataType.DateTime},
        };
    }

    public enum SimpleDataType
    {
        Undefined,
        Int,
        Decimal,
        Double,
        String,
        Bool,
        DateTime
    }
}
  