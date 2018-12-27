using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.Logic.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class DealScheduleLogicModel : BaseLogicModel
    {
        public DealScheduleLogicModel(ProjectLogicModel project)
            :base(project)
        {
            Initialize(project);
        }

        private void Initialize(ProjectLogicModel project)
        {
            var objStr = "产品 [" + project.Instance.Name + "] ";

            var projectId = project.Instance.ProjectId;
            if (project.Instance.Model == null && project.Instance.ModelId > 0)
            {
                project.Instance.Model = m_dbAdapter.Project.GetModel(project.Instance.ModelId);
            }

            var modelFolder = WebConfigUtils.RootFolder + project.Instance.Model.ModelFolder + "\\";
            CommUtils.Assert(Directory.Exists(modelFolder), "找不到模型路径[{0}]", modelFolder);

            DealSchedule dealSchedule = null;
            try
            {
                dealSchedule = NancyUtils.GetDealSchedule(projectId, ignoreException: false);
            }
            catch (Exception e)
            {
                CommUtils.Assert(false, "加载模型失败：[{0}]，模型路径：{1}"
                    + Environment.NewLine + e.Message, modelFolder);
            }
            CommUtils.AssertNotNull(dealSchedule, "加载模型失败：{0}", modelFolder);
            m_instanse = dealSchedule;

            CommUtils.Assert(DateUtils.IsNormalDate(dealSchedule.FirstCollectionPeriodStartDate),
                "模型中，FirstCollectionPeriodStartDate({0})不是有效的日期", dealSchedule.FirstCollectionPeriodStartDate.ToString());
            CommUtils.Assert(DateUtils.IsNormalDate(dealSchedule.FirstCollectionPeriodEndDate),
                "模型中，FirstCollectionPeriodEndDate({0})不是有效的日期", dealSchedule.FirstCollectionPeriodEndDate.ToString());

            CommUtils.AssertNotNull(dealSchedule.PaymentDates, objStr + "中没有PaymentDatas数据");
            CommUtils.AssertNotNull(dealSchedule.DeterminationDates, objStr + "中没有DeterminationDates数据");
            CommUtils.AssertEquals(dealSchedule.PaymentDates.Length, dealSchedule.DeterminationDates.Length, objStr + "中DealSchedule数据加载失败");

            _durationPeriods = new List<DatasetScheduleLogicModel>();
            for (int i = 0; i < dealSchedule.PaymentDates.Length; i++)
            {
                var paymentDate = dealSchedule.PaymentDates[i];
                var durationPeriod = new DatasetScheduleLogicModel(project);
                durationPeriod.Sequence = m_project.GetAllPaymentDates(dealSchedule.PaymentDates).IndexOf(paymentDate) + 1;
                durationPeriod.PaymentDate = paymentDate;
                if (i == 0)
                {
                    durationPeriod.AsOfDateBegin = dealSchedule.FirstCollectionPeriodStartDate;
                }
                else
                {
                    durationPeriod.AsOfDateBegin = dealSchedule.DeterminationDates[i - 1];
                }

                durationPeriod.AsOfDateEnd = dealSchedule.DeterminationDates[i];

                if (i != 0)
                {
                    durationPeriod.Previous = _durationPeriods[i - 1];
                    durationPeriod.Previous.Next = durationPeriod;
                }

                _durationPeriods.Add(durationPeriod);
            }

            ClosingDate = dealSchedule.ClosingDate;
            LegalMaturity = dealSchedule.LegalMaturity;
        }

        public DealSchedule Instanse
        {
            get
            {
                return m_instanse;
            }
        }

        private DealSchedule m_instanse;

        public DateTime ClosingDate { get; set; }

        public DateTime LegalMaturity { get; set; }

        public DatasetScheduleLogicModel GetByPaymentDay(DateTime date)
        {
            return _durationPeriods.FirstOrDefault(x => date <= x.PaymentDate) ?? _durationPeriods.Last();
        }

        public DatasetScheduleLogicModel GetByAsOfDate(DateTime date)
        {
            return _durationPeriods.FirstOrDefault(x => date <= x.AsOfDateEnd) ?? _durationPeriods.Last();
        }

        public DatasetScheduleLogicModel GetBySequence(int sequence)
        {
            var index = sequence - 1;
            if (_durationPeriods == null || 
                index < 0 || index >= _durationPeriods.Count)
            {
                return _durationPeriods[index];
            }

            return null;
        }

        public List<DatasetScheduleLogicModel> DurationPeriods
        {
            get
            {
                return _durationPeriods;
            }
        }

        private List<DatasetScheduleLogicModel> _durationPeriods;
    }
}
