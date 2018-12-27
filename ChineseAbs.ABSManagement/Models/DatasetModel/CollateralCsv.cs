using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class CollateralCsv : List<CollateralCsvRecord>
    {
        public CollateralCsv()
        {
        }

        private List<string> m_columnNames;

        public string FilePath { get; set; }

        public void Load(string filePath)
        {
            m_columnNames = new List<string>();

            var table = ExcelUtils.ReadCsv(filePath);
            for (int i = 0; i < table.Columns.Count; ++i)
            {
                m_columnNames.Add(table.Columns[i].ColumnName);
            }

            for (int i = 0; i < table.Rows.Count; ++i)
            {
                var record = new CollateralCsvRecord();
                bool isValidRecord = true;
                for (int j = 0; j < table.Columns.Count; ++j)
                {
                    var columnName = m_columnNames[j];
                    var cellValue = table.Rows[i].ItemArray[j].ToString();
                    if (columnName.Equals("AssetId", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(cellValue))
                        {
                            isValidRecord = false;
                            break;
                        }

                        record.AssetId = int.Parse(cellValue);
                    }
                    else
                    {
                        record.Items[columnName] = cellValue;
                    }
                }

                if (isValidRecord)
                {
                    this.Add(record);
                }
            }

            FilePath = filePath;
        }

        public bool ContainsColumn(string columnName)
        {
            return m_columnNames.Contains(columnName);
        }

        public void UpdateCellValue(int assetId, string name, string cellValue)
        {
            this.Where(x => x.AssetId == assetId && x.Items.ContainsKey(name)).ToList()
                .ForEach(x => x.Items[name] = cellValue);
        }

        public void Save(string filePath = null)
        {
            DataTable table = new DataTable();
            foreach (var columnName in m_columnNames)
            {
                table.Columns.Add(columnName);
            }

            foreach (var record in this)
            {
                DataRow row = table.NewRow();
                foreach (var columnName in m_columnNames)
                {
                    if (columnName.Equals("AssetId", StringComparison.InvariantCultureIgnoreCase))
                    {
                        row[columnName] = record.AssetId;
                    }
                    else
                    {
                        row[columnName] = record.Items[columnName];
                    }
                }

                table.Rows.Add(row);
            }


            if (string.IsNullOrEmpty(filePath))
            {
                filePath = FilePath;
            }

            ExcelUtils.WriteCsv(table, filePath);
        }
    }
}
