using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class PromisedCashflowCsv
    {
        string m_filename = "";
        public PromisedCashflowCsv(string filename)
        {
            m_filename = filename;
        }

        public void Load()
        {
            Records = new List<PromisedCashflowRecord>();
            if (!File.Exists(m_filename))
            {
                throw new FileNotFoundException(m_filename);
            }
            var datatable = ExcelUtils.ReadCsv(m_filename);
            foreach (DataRow row in datatable.Rows)
            {
                var assetid = row["AssetId"] == DBNull.Value ? "" : row["AssetId"].ToString();
                assetid = assetid.Trim();
                if (String.IsNullOrEmpty(assetid))
                    continue;
                Records.Add(new PromisedCashflowRecord()
                {
                    AssetId = row["AssetId"].ToString(),
                    PaymentDate = Convert.ToDateTime(row["PaymentDate"].ToString()),
                    Interest = Convert.ToDouble(row["Interest"]),
                    DefaultBalance = Convert.ToDouble(row["DefaultBalance"]),
                    Performing = Convert.ToDouble(row["Performing"]),
                    Principal = Convert.ToDouble(row["Principal"])
                });
            }
        }

        public void Save()
        {
            var datatable = new DataTable();

            datatable.Columns.Add("AssetId");
            datatable.Columns.Add("PaymentDate");
            datatable.Columns.Add("Interest");
            datatable.Columns.Add("Principal");
            datatable.Columns.Add("DefaultBalance");
            datatable.Columns.Add("Performing");

            foreach (var item in Records)
            {
                var row = datatable.NewRow();
                row["AssetId"] = item.AssetId.ToString();
                row["PaymentDate"] = item.PaymentDate.ToString("yyyy/M/dd");
                row["Interest"] = item.Interest.ToString();
                row["Principal"] = item.Principal.ToString();
                row["DefaultBalance"] = item.DefaultBalance.ToString();
                row["Performing"] = item.Performing.ToString();
                datatable.Rows.Add(row);
            }
            ExcelUtils.WriteCsv(datatable, m_filename);
        }

        public List<PromisedCashflowRecord> Records { get; set; }
    }
}
