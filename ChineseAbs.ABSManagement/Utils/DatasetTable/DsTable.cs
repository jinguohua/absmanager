using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils.DatasetTable
{
    public class DsTable
    {
        public DsTable(DataTable table)
        {
            ParseDataTable(table);
        }

        public DsRow FindRow(string description)
        {
            return Rows.FirstOrDefault(x => x.Description.Equals(description, StringComparison.CurrentCultureIgnoreCase));
        }

        public void RemoveRow(List<DsRow> rows)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row != null)
                {
                    Rows.RemoveAll(x => x.Name.Equals(row.Name, StringComparison.CurrentCultureIgnoreCase)
                        && x.Description.Equals(row.Description, StringComparison.CurrentCultureIgnoreCase));
                }
            }
        }

        public void RemoveRow(DsRow row)
        {
            if (row != null)
            {
                Rows.RemoveAll(x => x.Name.Equals(row.Name, StringComparison.CurrentCultureIgnoreCase)
                    && x.Description.Equals(row.Description, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        private void ParseDataTable(DataTable table)
        {
            ColumnCount = table.Columns.Count;

            var principalReceivedKey = ".Principal Received";

            Notes = new List<string>();
            Rows = new List<DsRow>();
            for (int i = 0; i < table.Rows.Count; ++i)
            {
                var row = table.Rows[i];
                var dsRow = new DsRow(row);
                Rows.Add(dsRow);

                if (dsRow.Description.EndsWith(principalReceivedKey, StringComparison.CurrentCultureIgnoreCase))
                {
                    var noteName = dsRow.Description.Substring(0, dsRow.Description.Length - principalReceivedKey.Length);
                    Notes.Add(noteName);
                }
            }
        }

        public List<string> Notes { get; private set; }

        public List<DsRow> Rows { get; private set; }

        public int ColumnCount { get; private set; }

        public void OverrideTo(DataTable table)
        {
            table.Clear();
            foreach (var dsRow in Rows)
            {
                var row = table.NewRow();
                for (int i = 0; i < dsRow.Items.Count; ++i)
                {
                    row[i] = dsRow.Items[i];
                }
                table.Rows.Add(row);
            }
        }
    }
}
