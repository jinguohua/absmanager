using System.Data;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils
{
    public class NancyMockUtils
    {
        public static void SetOverridableVariables(DataSet ds, string dsFolder)
        {
            string futureVar = dsFolder + "\\FutureVariables.csv";
            string currVar = dsFolder + "\\CurrentVariables.csv";
            if (!File.Exists(futureVar))
            {
                return;
            }
            DataTable dtFuture = ExcelUtils.ReadCsv(futureVar);
            DataTable dtCurrent = ExcelUtils.ReadCsv(currVar);

            var inDtCurrent = ds.Tables[0];
            var inDtFuture = ds.Tables[1];

            if (inDtFuture.Columns.Count != dtFuture.Columns.Count)
            {
                return;
            }

            foreach (DataRow row in dtFuture.Rows)
            {
                if (inDtFuture.AsEnumerable().Any(r => r["Name"].ToString().Equals(row["Name"].ToString())))
                {
                    DataRow dr = inDtFuture.AsEnumerable().FirstOrDefault(r => r["Name"].ToString().Equals(row["Name"].ToString()));
                    for (int i = 0; i <= inDtFuture.Columns.Count - 1; i++)
                    {
                        if ((dr[i].ToString() != row[i].ToString() && dr[i].ToString() != string.Empty))
                        {
                            row[i] = dr[i];
                        }
                    }
                }
            }

            foreach (DataRow row in dtCurrent.Rows)
            {
                if (inDtCurrent.AsEnumerable().Any(r => r["Name"].ToString().Equals(row["Name"].ToString())))
                {
                    DataRow dr = inDtCurrent.AsEnumerable().FirstOrDefault(r => r["Name"].ToString().Equals(row["Name"].ToString()));
                    for (int i = 0; i <= inDtCurrent.Columns.Count - 1; i++)
                    {
                        if ((dr[i].ToString() != row[i].ToString() && dr[i].ToString() != string.Empty))
                        {
                            row[i] = dr[i];
                        }
                    }
                }
            }
            ExcelUtils.WriteCsv(dtFuture, Path.Combine(dsFolder, "FutureVariables.csv"));
            ExcelUtils.WriteCsv(dtCurrent, Path.Combine(dsFolder, "CurrentVariables.csv"));
        }
    }
}
