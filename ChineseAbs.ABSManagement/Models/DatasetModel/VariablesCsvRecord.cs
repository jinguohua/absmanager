using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class VariablesCsvRecord
    {
        public VariablesCsvRecord()
        {
            Items = new Dictionary<DateTime, string>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<DateTime, string> Items { get; set; }
    }
}
