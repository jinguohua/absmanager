using CNABS.Mgr.Deal.Model.Setting;
using ChineseAbs.Logic.Object;
using System;
using System.Linq;
using CNABS.Mgr.Deal.Utils;
using ChineseAbs.ABSManagement;
using CNABS.Mgr.Deal.Model.Result;
using ChineseAbs.CalcService.Data.NancyData;
using System.Collections.Generic;
using ChineseAbs.ABSManagement.LogicModels.DealModel;

namespace CNABS.Mgr.Deal.Model
{
    /// <summary>
    /// ABS产品模型
    /// </summary>
    public class ABSDeal
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ABSDeal(string ymlFolder, string dsFolder)
        {
            Location = new Location(ymlFolder, dsFolder);
            Setting = new ABSDealSetting();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ABSDeal(DealLogicModel dealLogicModel)
        {
            Location = new Location(dealLogicModel.YmlFolder, dealLogicModel.DsFolder);
            Setting = new ABSDealSetting();
            m_schedule.Get(() => dealLogicModel.ProjectLogicModel.DealSchedule.Instanse);
        }

        /// <summary>
        /// 模型相关配置/参数
        /// </summary>
        public ABSDealSetting Setting { get; set; }

        /// <summary>
        /// 运行现金流结果
        /// </summary>
        public ABSDealResult Result
        {
            get
            {
                return m_result.Get(() => ABSDealUtils.GetNancyDealResult(this));
            }
        }

        private LazyConstruct<ABSDealResult> m_result;

        public Assets Assets
        {
            get
            {
                return m_assets.Get(() => {
                    Location.Validate();
                    return Assets.Load(this);
                });
            }
        }

        private LazyConstruct<Assets> m_assets;

        /// <summary>
        /// 偿付日
        /// </summary>
        public DateTime PaymentDay { get; set; }

        /// <summary>
        /// 上一期模型
        /// </summary>
        public ABSDeal Previous { get; set; }

        /// <summary>
        /// 下一期模型
        /// </summary>
        public ABSDeal Next { get; set; }

        /// <summary>
        /// DealModel的路径相关信息
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// 根据yml文件读取的deal基本信息
        /// </summary>
        public NancyDealData Info
        {
            get
            {
                return m_info.Get(() => {
                    var info = ABSDealUtils.GetNancyDealData(this);
                    FirstCollectionPeriodStartDate = info.FirstCollectionPeriodStartDate;
                    return info.Info;
                    });
            }
        }

        private DateTime? FirstCollectionPeriodStartDate;

        public DealSchedule DealSchedule
        {
            get
            {
                if (m_dealSchedule == null)
                {
                    m_dealSchedule = new DealSchedule();
                    var scheduleData = Info.ScheduleData;
                    //第一个偿付期的计息区间起始日期就是起息日
                    m_dealSchedule.FirstAccrualDate = scheduleData.ClosingDate;
                    m_dealSchedule.ClosingDate = scheduleData.ClosingDate;
                    m_dealSchedule.LegalMaturity = scheduleData.LegalMaturity;
                    m_dealSchedule.PaymentDates = scheduleData.PaymentSchedule.Periods.Select(x => x.PaymentDate).ToArray();
                    m_dealSchedule.DeterminationDates = scheduleData.PaymentSchedule.Periods.Select(x => x.DeterminationDate).ToArray();

                    m_dealSchedule.FirstCollectionPeriodStartDate = FirstCollectionPeriodStartDate.HasValue
                        ? FirstCollectionPeriodStartDate.Value
                        : scheduleData.PaymentSchedule.Periods.First().CollectionBegin;

                    m_dealSchedule.FirstCollectionPeriodEndDate = scheduleData.PaymentSchedule.Periods.First().CollectionEnd;

                    var scheduledPaymentDateArray = Info.ScheduleData.PaymentSchedule.Periods.Select(x => x.ScheduledPaymentDate).ToArray();
                    m_dealSchedule.NoteAccrualDates = new Dictionary<string, DateTime[]>();
                    Info.Notes.ForEach(x => m_dealSchedule.NoteAccrualDates[x.Name] = scheduledPaymentDateArray);
                }

                return m_dealSchedule;
            }
        }

        private DealSchedule m_dealSchedule = null;

        private LazyConstruct<NancyDealData> m_info;

        public DealSchedule Schedule
        {
            get
            {
                return m_schedule.Get(() => ABSDealUtils.GetNancyDealSchedule(this));
            }
        }

        private LazyConstruct<DealSchedule> m_schedule;
    }
}
