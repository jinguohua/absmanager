using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class DatasetScheduleLogicModel : BaseLogicModel
    {
        public DatasetScheduleLogicModel(ProjectLogicModel project)
            : base(project)
        {
        }

        public int Sequence { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime AsOfDateBegin { get; set; }

        public DateTime AsOfDateEnd { get; set; }

        public DatasetScheduleLogicModel Previous { get; set; }

        public DatasetScheduleLogicModel Next { get; set; }

        public List<DateTime> SelectPaymentDates(Func<DatasetScheduleLogicModel, bool> filter)
        {
            List<DateTime> paymentDates = new List<DateTime>();

            var datasetSchedule = this;
            while (datasetSchedule.Previous != null)
            {
                datasetSchedule = datasetSchedule.Previous;
            }

            while (datasetSchedule != null)
            {
                if (filter(datasetSchedule))
                {
                    paymentDates.Add(datasetSchedule.PaymentDate);
                }
                datasetSchedule = datasetSchedule.Next;
            }

            return paymentDates;
        }

        public DatasetLogicModel Dataset
        {
            get { 
                if (m_dataset == null)
                {
                    m_dataset = new DatasetLogicModel(this);
                }
                return m_dataset;
            }
        }

        private DatasetLogicModel m_dataset;
    }
}
