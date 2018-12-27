using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class SimpleScheduleData
    {
        public SimpleScheduleData() { }

        public SimpleScheduleData(DateTime[] dates, double[] values)
        {

            this.Dates = dates;
            this.Values = values;
        }

        public int Count()
        {
            return IsValid ? this.Dates.Length : 0;
        }

        public bool IsValid
        {
            get
            {
                if (this.Dates == null || this.Values == null)
                {
                    return false;
                }
                return this.Dates.Length == this.Values.Length;
            }
        }



        public void Add(DateTime date, double value)
        {
            if (Dates == null || Values == null)
            {
                Dates = new DateTime[1];
                Values = new double[1];
            }
            else
            {
                Array.Resize(ref m_dates, m_dates.Length + 1);
                Array.Resize(ref m_values, m_values.Length + 1);
            }
            m_dates[m_dates.Length - 1] = date;
            m_values[m_values.Length - 1] = value;
        }

        public double GetLeftValue(DateTime date)
        {
            for (int i = m_dates.Length - 1; i >= 0; i--)
            {
                if (date >= m_dates[i])
                {
                    return m_values[i];
                }
            }
            return 0.0;
        }

        public DateTime[] Dates
        {
            get { return m_dates; }
            set { m_dates = value; }
        }

        public double[] Values
        {
            get { return m_values; }
            set { m_values = value; }
        }

        private DateTime[] m_dates;
        private double[] m_values;
    }
}
