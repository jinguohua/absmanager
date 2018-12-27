using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    //public class VariablesCsv : List<VariablesCsvRecord>
    //{
    //    public VariablesCsv()
    //    {
    //        DateColumns = new List<DateTime>();
    //    }

    //    public List<DateTime> DateColumns { get; set; }

    //    public Dictionary<string, string> GetVariablesByDate(DateTime dateTime)
    //    {
    //        var dict = new Dictionary<string, string>();
    //        foreach (var record in this)
    //        {
    //            if (record.Items.ContainsKey(dateTime))
    //            {
    //                dict[record.Name] = record.Items[dateTime];
    //            }
    //        }
    //        return dict;
    //    }

    //    private string m_filePath;

    //    public void Load(string filePath)
    //    {
    //        Clear();
    //        DateColumns.Clear();

    //        var table = ExcelUtils.ReadCsv(filePath);
    //        CommUtils.Assert(table.Columns.Count >= 2, "Parse file [" + filePath + "] failed.");

    //        var nameColumnIndex = -1;//第一行名称为Name的列index
    //        var descriptionColumnIndex = -1;//第一行名称为Description的列index

    //        //Column Index & Payment date
    //        var colIndexDates = new List<Tuple<int, DateTime>>();


    //        for (int i = 0; i < table.Columns.Count; ++i)
    //        {
    //            var colName = table.Columns[i].ColumnName;
    //            if (nameColumnIndex == -1 &&
    //                colName.Equals("Name", StringComparison.CurrentCultureIgnoreCase))
    //            {
    //                nameColumnIndex = i;
    //            }

    //            if (descriptionColumnIndex == -1 &&
    //                colName.Equals("Description", StringComparison.CurrentCultureIgnoreCase))
    //            {
    //                descriptionColumnIndex = i;
    //            }

    //            DateTime date;
    //            if (DateTime.TryParse(colName, out date))
    //            {
    //                colIndexDates.Add(Tuple.Create(i, date));
    //                DateColumns.Add(date);
    //            }
    //        }

    //        for (int i = 0; i < table.Rows.Count; ++i)
    //        {
    //            var row = table.Rows[i];
    //            var name = row.ItemArray[nameColumnIndex].ToString();
    //            var description = row.ItemArray[descriptionColumnIndex].ToString();

    //            var record = new VariablesCsvRecord();
    //            record.Name = name;
    //            record.Description = description;

    //            foreach (var colIndexDate in colIndexDates)
    //            {
    //                var colIndex = colIndexDate.Item1;
    //                var date = colIndexDate.Item2;
    //                var cellValue = row.ItemArray[colIndex].ToString();
    //                record.Items[date] = cellValue;
    //            }

    //            Add(record);
    //        }

    //        m_filePath = filePath;
    //    }

    //    public void RemoveColumn(DateTime colDate)
    //    {
    //        var index = DateColumns.IndexOf(colDate);
    //        if (index < 0)
    //        {
    //            return;
    //        }

    //        DateColumns.Remove(colDate);

    //        foreach (var record in this)
    //        {
    //            if (record.Items.ContainsKey(colDate))
    //            {
    //                record.Items.Remove(colDate);
    //            }
    //        }
    //    }

    //    public void UpdateColumnDate(DateTime oldColDate, DateTime newColDate)
    //    {
    //        if (oldColDate == newColDate)
    //        {
    //            return;
    //        }

    //        var index = DateColumns.IndexOf(oldColDate);
    //        CommUtils.Assert(index >= 0, "Search column [" + oldColDate.ToString() + "] failed.");
    //        DateColumns[index] = newColDate;

    //        foreach (var record in this)
    //        {
    //            if (record.Items.ContainsKey(oldColDate))
    //            {
    //                record.Items[newColDate] = record.Items[oldColDate];
    //                record.Items.Remove(oldColDate);
    //            }
    //        }
    //    }

    //    public void UpdateCellValue(string name, DateTime colDate, string cellValue)
    //    {
    //        CommUtils.Assert(DateColumns.Contains(colDate), "Search column [" + colDate.ToString() + "] failed.");

    //        foreach (var record in this)
    //        {
    //            if (record.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
    //            {
    //                record.Items[colDate] = cellValue;
    //            }
    //        }
    //    }

    //    public void Save(string filePath = null)
    //    {
    //        DataTable table = new DataTable();
    //        table.Columns.Add("Name");
    //        table.Columns.Add("Description");
    //        this.DateColumns.ForEach(x => table.Columns.Add(x.ToString("MM/dd/yyyy")));

    //        foreach (var record in this)
    //        {
    //            DataRow row = table.NewRow();
    //            row["Name"] = record.Name;
    //            row["Description"] = record.Description;

    //            foreach (var column in this.DateColumns)
    //            {
    //                var key = column.ToString("MM/dd/yyyy");
    //                row[key] = record.Items.ContainsKey(column) ? record.Items[column] : string.Empty;
    //            }
    //            table.Rows.Add(row);
    //        }

    //        if (string.IsNullOrEmpty(filePath))
    //        {
    //            filePath = m_filePath;
    //        }

    //        ExcelUtils.WriteCsv(table, filePath);
    //    }
    //}

    public class VariablesHelper
    {


        public class VariablesInfo
        {
            public List<VariablesCsvRecord> Variables { get; set; } = new List<VariablesCsvRecord>();

            public List<DateTime> Dates { get; set; } = new List<DateTime>();
        }


        private readonly string pastVariablesFile = "PastVariables.csv";
        private readonly string currentVariablesFile = "CurrentVariables.csv";
        private readonly string futureVariablesFile = "FutureVariables.csv";
        private readonly string combileVariablesFile = "CombinedVariables.csv";
        private string m_dsFolder = "";
        private DateTime m_asofdate;
        private bool m_isCombile = false;
        private string asofdateVal = "SimulationBegin";

        VariablesInfo Variables { get; set; }

        public DateTime Asofdate
        {
            get
            {
                return m_asofdate;
            }
        }

        public List<DateTime> DateColumns
        {
            get
            {
                if (Variables != null)
                    return Variables.Dates;
                else
                    return new List<DateTime>();
            }
        }

        public VariablesHelper(string dsFolder)
            : this(dsFolder, null)
        {

        }

        public VariablesHelper(string dsFolder, DateTime? asofdate)
        {
            m_dsFolder = dsFolder;
            if (!Directory.Exists(dsFolder))
                throw new DirectoryNotFoundException(dsFolder);
            if (!asofdate.HasValue)
            {
                var folder = new DirectoryInfo(dsFolder).Name;
                int dateValue = 0;
                if (int.TryParse(folder, out dateValue))
                {
                    int year = dateValue / 10000;
                    int month = (dateValue % 10000) / 100;
                    int day = (dateValue % 100);
                    try
                    {
                        asofdate = new DateTime(year, month, day);
                    }
                    catch
                    {

                    }
                }
                if (asofdate == null)
                    throw new Exception("Asofdate 为空");
                else
                    m_asofdate = asofdate.Value;
            }
            CheckFiles(true);
            var combilePath = Path.Combine(m_dsFolder, combileVariablesFile);

            m_isCombile = File.Exists(combilePath);
        }

        public void Load(bool checkAsofdate = false)
        {
            VariablesInfo result = null;
            if (m_isCombile)
            {
                result = LoadFromFile(Path.Combine(m_dsFolder, combileVariablesFile));
            }
            else
            {
                result = Load3Files();
                Save();
                m_isCombile = true;
            }
                

            var asofdateItem = result.Variables.FirstOrDefault(o => o.Name.Equals(asofdateVal, StringComparison.CurrentCultureIgnoreCase));
            if (asofdateItem != null)
            {
                var obj = asofdateItem.Items.FirstOrDefault(o => o.Value.Equals("TRUE", StringComparison.CurrentCultureIgnoreCase));
                if (obj.Value != null)
                {
                    if (checkAsofdate && m_asofdate != obj.Key)
                    {
                        throw new Exception("变量： SimulationBegin的值和当前的asofdate不相符");
                    }
                    m_asofdate = obj.Key;
                }
                result.Variables.Remove(asofdateItem);
            }
            Variables = result;
        }

        private VariablesInfo Load3Files()
        {
            VariablesInfo result = new VariablesInfo();
            VariablesInfo result1 = LoadFromFile(pastVariablesFile);
            VariablesInfo result2 = LoadFromFile(currentVariablesFile);
            if (result2.Dates == null || result2.Dates.Count == 0)
            {
                throw new Exception("无法加载: currentVariablesFile");
            }
            m_asofdate = result2.Dates[0];
            VariablesInfo result3 = LoadFromFile(futureVariablesFile);

            List<VariablesCsvRecord> items = new List<VariablesCsvRecord>();
            items.AddRange(result1.Variables);
            items.AddRange(result2.Variables);
            items.AddRange(result3.Variables);

            items = items.GroupBy(o => new { o.Name }).Select(o =>
             {
                 var item = new VariablesCsvRecord
                 {
                     Name = o.Key.Name,
                     Description = o.First().Description
                 };
                 var values = o.SelectMany(m => m.Items);
                 foreach (var v in values)
                 {
                     if (!item.Items.ContainsKey(v.Key))
                     {
                         item.Items.Add(v.Key, v.Value);
                     }
                 }
                 return item;
             }).ToList();
            result.Variables = items;
            result.Dates.AddRange(result1.Dates);
            result.Dates.AddRange(result2.Dates);
            result.Dates.AddRange(result3.Dates);
            result.Dates = result.Dates.Distinct().OrderBy(o => o).ToList();
            return result;
        }

        private VariablesInfo LoadFromFile(string filename)
        {
            var filepath = Path.Combine(m_dsFolder, filename);
            DataTable dt = ExcelUtils.ReadCsv(filepath);
            DateTime d;
            List<VariablesCsvRecord> values = new List<VariablesCsvRecord>();
            var columns = dt.Columns.Cast<DataColumn>().Select(o => o.ColumnName.Trim().ToLower())
                .Select(o => DateTime.TryParse(o, out d) ? o : "")
                .Where(o => !String.IsNullOrEmpty(o))
                .ToList();

            foreach (DataRow row in dt.Rows)
            {
                var name = row.Field<string>("Name");
                var des = row.Field<string>("Description");
                if (String.IsNullOrEmpty(name))
                    continue;
                VariablesCsvRecord item = new VariablesCsvRecord()
                {
                    Name = name,
                    Description = des
                };
                values.Add(item);
                foreach (var col in columns)
                {
                    var date = Convert.ToDateTime(col);
                    string value = row[col] == DBNull.Value ? "" : row[col].ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        item.Items.Add(date, value);
                    }
                }
            }
            return new VariablesInfo { Variables = values, Dates = columns.Select(o => Convert.ToDateTime(o)).ToList() };
        }

        public void Save(string folder = null)
        {
            var dateFormat = @"MM/dd/yyyy";
            if (Variables == null || Variables.Dates.Count == 0)
                return;
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Description");
            foreach (var col in Variables.Dates)
            {
                dt.Columns.Add(col.ToString(dateFormat));
            }
            foreach (var item in Variables.Variables)
            {
                var row = dt.NewRow();
                dt.Rows.Add(row);
                row["Name"] = item.Name;
                row["Description"] = item.Description;
                foreach (var val in item.Items)
                {
                    var c = val.Key.ToString(dateFormat);
                    if (dt.Columns.Contains(c))
                        row[c] = val.Value;
                }
            }

            var endRow = dt.NewRow();
            dt.Rows.Add(endRow);
            endRow["Name"] = asofdateVal;
            var asofdateCol = m_asofdate.ToString(dateFormat);
            if (!dt.Columns.Contains(asofdateCol))
                throw new Exception("无法设置asofdate:" + asofdateCol);
            endRow[asofdateCol] = "TRUE";
            if (string.IsNullOrEmpty(folder))
                folder = m_dsFolder;
            string filepath = Path.Combine(folder, combileVariablesFile);
            ExcelUtils.WriteCsv(dt, filepath);
        }

        public void RemoveDates(params DateTime[] dates)
        {
            if (dates != null)
            {
                if (Variables != null)
                {
                    foreach (var date in dates)
                        Variables.Dates.Remove(date);
                }
            }
        }

        public void RemoveVariable(string name)
        {
            var item = Variables.Variables.FirstOrDefault(o => o.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (item != null)
                Variables.Variables.Remove(item);
        }

        public void UpdateVariableValue(string name, DateTime d, string value, string description = null)
        {
            var item = Variables.Variables.FirstOrDefault(o => o.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (item == null)
            {
                item = new VariablesCsvRecord { Name = name, Description = description ?? name };
                Variables.Variables.Add(item);
            }
            if (item.Items.ContainsKey(d))
                item.Items[d] = value;
            else
                item.Items.Add(d, value);
        }

        public Dictionary<string, string> GetVariablesByDate(DateTime dateTime)
        {
            return Variables.Variables.Where(o => o.Items.ContainsKey(dateTime))
                .Select(o => new { key = o.Name, value = o.Items[dateTime] })
                .ToDictionary(o => o.key, o => o.value, StringComparer.CurrentCultureIgnoreCase);
        }

        public string GetVariable(string name, DateTime? datetime = null)
        {
            if (!datetime.HasValue)
                datetime = m_asofdate;
            var vitem = Variables.Variables.FirstOrDefault(o => o.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (vitem == null || !vitem.Items.ContainsKey(datetime.Value))
                return "";
            else
                return vitem.Items[datetime.Value];
        }

        public T GetVariable<T>(string name, DateTime? datetime = null)
        {
            string v = GetVariable(name, datetime);
            try
            {
                return (T)Convert.ChangeType(v, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public List<VariablesCsvRecord> GetVariablesByDateRange(DateTime? start, DateTime? end)
        {
            if (start == null)
                start = DateTime.MinValue;
            if (end == null)
                end = DateTime.MaxValue;
            return Variables.Variables.Select(o => 
                new VariablesCsvRecord {
                    Name = o.Name,
                    Description = o.Description,
                    Items = o.Items.Where(m => m.Key >= start.Value && m.Key < end.Value).ToDictionary(m => m.Key, m => m.Value)
                })
                .Where(o => o.Items != null && o.Items.Count > 0).ToList();
        }

        public List<VariablesCsvRecord> FutureVariables
        {
            get
            {
                return GetVariablesByDateRange(m_asofdate, null);
            }
        }

        public List<VariablesCsvRecord> PastVariables
        {
            get
            {
                return GetVariablesByDateRange(null, m_asofdate.AddDays(-1));
            }
        }

        public Dictionary<string, string> CurrentVariables
        {
            get
            {
                return GetVariablesByDate(m_asofdate);
            }
        }

        public void RemoveVariables(DateTime date, string[] names = null)
        {
            var vs = Variables.Variables;
            if(names != null && names.Length > 0)
            {
                vs = vs.Where(o => names.Contains(o.Name, StringComparer.CurrentCultureIgnoreCase)).ToList();
            }
            foreach (var va in vs)
            {
                if (va.Items.ContainsKey(date))
                {
                    va.Items.Remove(date);
                }
            }
        }

        public void TranferToAsofdate(DateTime asofdate)
        {
            m_asofdate = asofdate;
            var dates = Variables.Dates.Where(o => o < asofdate);
            foreach(var date in dates)
            {
                RemoveVariables(date);
            }
        }

        public bool CheckFiles(bool throwException)
        {
            bool flag = true;
            flag = File.Exists(Path.Combine(m_dsFolder, combileVariablesFile));
            if (!flag)
            {
                flag = File.Exists(Path.Combine(m_dsFolder, pastVariablesFile))
                    && File.Exists(Path.Combine(m_dsFolder, currentVariablesFile))
                    && File.Exists(Path.Combine(m_dsFolder, futureVariablesFile));
            }

            if (!flag && throwException)
            {
                throw new FileNotFoundException($"目录{m_dsFolder}下变量文件缺失");
            }

            return flag;
        }

        public List<InterestRateReset> GetInterestRateResets()
        {
            var varCsv = this;
            var keywordPostfix = ".reset";
            PrimeInterestRate rateType;
            var interestRateRecords = Variables.Variables.Where(x =>
                x.Name.EndsWith(keywordPostfix, StringComparison.CurrentCultureIgnoreCase) &&
                Enum.TryParse<PrimeInterestRate>(x.Name.Substring(0, x.Name.Length - keywordPostfix.Length), true, out rateType)).ToList();

            var nowDate = DateTime.Today;
            var result = new List<InterestRateReset>();
            foreach (var record in interestRateRecords)
            {
                Enum.TryParse<PrimeInterestRate>(record.Name.Substring(0, record.Name.Length - keywordPostfix.Length), true, out rateType);

                var rateReset = new InterestRateReset();
                rateReset.Code = rateType;

                foreach (var paymentDate in varCsv.DateColumns)
                {
                    var rateValue = MathUtils.ParseDouble(record.Items[paymentDate]);
                    if (rateReset.ResetRecords.Count == 0 || rateReset.ResetRecords.Last().InterestRate != rateValue)
                    {
                        var resetRecord = new InterestRateResetRecord();
                        resetRecord.Date = paymentDate;
                        resetRecord.InterestRate = rateValue;
                        rateReset.ResetRecords.Add(resetRecord);
                    }
                }

                //查找第一个不大于今天的的利率作为今天利率
                rateReset.CurrentInterestRate = rateReset.ResetRecords.First().InterestRate;
                foreach (var resetRecord in rateReset.ResetRecords)
                {
                    if (resetRecord.Date < nowDate)
                    {
                        rateReset.CurrentInterestRate = resetRecord.InterestRate;
                    }
                }

                result.Add(rateReset);
            }

            return result;
        }
    }
}