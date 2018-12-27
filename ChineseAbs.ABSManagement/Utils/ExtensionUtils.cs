using ChineseAbs.CalcService.Data.NancyData.Cashflows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class ExtensionUtils
    {
        public static List<NancyBasicAssetCashflowItem> SelectByAsset(
            this List<NancyBasicAssetCashflowItem> records, int assetId, bool ignoreZeroPrincipal = true)
        {
            var items = records.Where(x => x.AssetId == assetId);
            if (ignoreZeroPrincipal)
            {
                items = items.Where(x => x.Principal != 0);
            }
            return items.OrderBy(x => x.PaymentDate).ToList();
        }

        public static int IndexOfTable(this DataSet dataSet, Func<DataTable, bool> filter)
        {
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                var table = dataSet.Tables[i];
                if (filter(table))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int IndexOfRow(this DataTable table, Func<DataRow, bool> filter)
        {
            for (int i = 0; i < table.Rows.Count; ++i)
            {
                var row = table.Rows[i];
                if (filter(row))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void RemoveAllRow(this DataTable table, Func<DataRow, bool> filter)
        {
            for (int i = table.Rows.Count - 1; i >= 0; --i)
            {
                var row = table.Rows[i];
                if (filter(row))
                {
                    table.Rows.RemoveAt(i);
                }
            }
        }

        public static void CopyRow(this DataTable table, int srcIndex, int destIndex, int startColumnIndex)
        {
            var srcRow = table.Rows[srcIndex];
            var destRow = table.Rows[destIndex];
            for (int i = startColumnIndex; i < srcRow.ItemArray.Length; i++)
            {
                destRow[i] = srcRow[i].ToString();
            }
        }

        public static List<List<string>> ToListList(this DataTable dt)
        {
            var result = new List<List<string>>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                result.Add(dt.Rows[i].ItemArray.ToList().ConvertAll(x => x.ToString()));
            }
            return result;
        }

        public static object ToHandson(this DataTable dt)
        {
            var columnNames = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
			{
                columnNames.Add(dt.Columns[i].ColumnName);
			}

            return new
            {
                colHeader = columnNames,
                dataResult = dt.ToListList(),
            };
        }

        public static string ToJson(this DataTable dt)
        {
            StringBuilder json = new StringBuilder();
            json.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                json.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    var columnName = dt.Columns[j].ColumnName;
                    var cellText = dt.Rows[i][j].ToString();
                    json.Append("\"" + columnName + "\":" + "\"" + cellText + "\"");
                    if (j < dt.Columns.Count - 1)
                    {
                        json.Append(",");
                    }
                }
                json.Append("}");
                if (i < dt.Rows.Count - 1)
                {
                    json.Append(",");
                }
            }
            json.Append("]");
            return json.ToString();
        }

        public static DataTable ToDataTable(this string json)
        {
            DataTable dataTable = new DataTable();
            DataTable result;

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.MaxJsonLength = Int32.MaxValue;
            ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);

            if (arrayList.Count > 0)
            {
                foreach (Dictionary<string, object> dictionary in arrayList)
                {
                    if (dictionary.Keys.Count<string>() == 0)
                    {
                        result = dataTable;
                        return result;
                    }
                    if (dataTable.Columns.Count == 0)
                    {
                        foreach (string current in dictionary.Keys)
                        {
                            dataTable.Columns.Add(current, dictionary[current].GetType());
                        }
                    }
                    DataRow dataRow = dataTable.NewRow();
                    foreach (string current in dictionary.Keys)
                    {
                        dataRow[current] = dictionary[current];
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            result = dataTable;
            return result;
        }

        public static DataTable Transpose(this DataTable dt)
        {
            int dtColumnsNum = dt.Columns.Count;
            int dtRowsNum = dt.Rows.Count;
            DataTable dtRes = new DataTable();

            for (int i = 0; i <= dtRowsNum; i++)
            {
                dtRes.Columns.Add("n" + i);
            }
            for (int i = 0; i < dtColumnsNum; i++)
            {
                ArrayList a = new ArrayList(dtRowsNum + 1);
                for (int j = 0; j <= dtRowsNum; j++)
                {
                    if (j == 0)
                    {
                        a.Add(dt.Columns[i].ColumnName.ToString());
                    }
                    else
                    {
                        a.Add(dt.Rows[j - 1][i].ToString());
                    }
                }
                dtRes.Rows.Add(a.ToArray());
            }

            return dtRes;
        }

        public static double SumColumn(this DataTable dt, string columnName, int rowIndexBegin, int rowCount)
        {
            double sum = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i >= rowIndexBegin && i < rowIndexBegin + rowCount)
                {
                    double value = 0;
                    if (double.TryParse(dt.Rows[i][columnName].ToString(), out value))
                    {
                        sum += value;
                    }
                }
            }

            return sum;
        }

        public static MemoryStream ToCsvMemoryStream(this DataTable dt, string currentUserName)
        {
            var temproaryFolder = CommUtils.CreateTemporaryFolder(currentUserName);
            var filePath = Path.Combine(temproaryFolder, "Temporary.csv");
            ExcelUtils.WriteCsv(dt, filePath);

            var buffer = System.IO.File.ReadAllBytes(filePath);
            CommUtils.DeleteFolderAync(temproaryFolder);

            return new MemoryStream(buffer);
        }

        public static Dictionary<TKey, List<TValue>> GroupToDictList<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        {
            return source.GroupBy(keySelector).ToDictionary(x => x.Key, y => y.ToList());
        }
    }
}
