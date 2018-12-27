using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils.DatasetTable
{
    public class DsRow
    {
        public DsRow()
        {
            m_items = new List<string> { "SysGen_Name", "SysGen_Description" };
        }

        public DsRow(DataRow row)
        {
            m_items = row.ItemArray.Select(x => x.ToString()).ToList();
        }

        public string this[int columnIndex] { get { return m_items[columnIndex]; } }

        public string Name { get { return m_items[0]; } set { m_items[0] = value.ToString(); } }

        public string Description { get { return m_items[1]; } set { m_items[1] = value.ToString(); } }

        public void AppendValue(List<string> values)
        {
            m_items.AddRange(values);
        }

        public void AppendValue(string value)
        {
            m_items.Add(value);
        }

        /// <summary>
        /// 复制指定行除 Name,Description 外的字段
        /// </summary>
        /// <param name="dsRow"></param>
        public void CopyValueFrom(DsRow dsRow)
        {
            var name = Name;
            var description = Description;
            m_items.Clear();
            m_items.Add(name);
            m_items.Add(description);
            for (int i = 2; i < dsRow.m_items.Count; i++)
            {
                m_items.Add(dsRow.m_items[i]);
            }
        }

        public static DsRow operator + (DsRow l, DsRow r)
        {
            return BinaryMathOperate(l, r, (x, y) => x + y);
        }

        public static DsRow operator - (DsRow l, DsRow r)
        {
            return BinaryMathOperate(l, r, (x, y) => x - y);
        }

        public static DsRow operator * (DsRow l, DsRow r)
        {
            return BinaryMathOperate(l, r, (x, y) => x * y);
        }

        public static DsRow BinaryMathOperate(DsRow l, DsRow r, Func<decimal, decimal, decimal> func)
        {
            var dsRow = new DsRow();
            for (int i = 2; i < l.m_items.Count && i < r.m_items.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(l.m_items[i])
                    || string.IsNullOrWhiteSpace(r.m_items[i]))
                {
                    dsRow.m_items.Add(string.Empty);
                }
                else
                {
                    decimal lValue = 0;
                    decimal rValue = 0;
                    CommUtils.Assert(decimal.TryParse(l.m_items[i], out lValue), "处理DatasetRow过程中，无法将{0}转换为数值", l.m_items[i]);
                    CommUtils.Assert(decimal.TryParse(r.m_items[i], out rValue), "处理DatasetRow过程中，无法将{0}转换为数值", r.m_items[i]);

                    var result = func(lValue, rValue);
                    dsRow.m_items.Add(result.ToString("n2"));
                }
            }

            return dsRow;
        }

        public List<string> Items { get { return m_items; } }

        private List<string> m_items;
    }
}
