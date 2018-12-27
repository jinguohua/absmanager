using ChineseAbs.ABSManagement.Manager;
using ChineseAbs.ABSManagement.Manager.AssetDefault;
using ChineseAbs.ABSManagement.Manager.DocumentManagementSystem;
using ChineseAbs.ABSManagement.Manager.InterestRateAdjustment;
using ChineseAbs.ABSManagement.Manager.Prepayment;
using ChineseAbs.ABSManagement.Manager.TimeRuleManager;
using ChineseAbs.ABSManagement.Manager.MessageReminding;
using ChineseAbs.ABSManagement.Manager.DealModel;
using ChineseAbs.ABSManagement.Manager.Email;

namespace ChineseAbs.ABSManagement
{
    public class DBAdapter: SAFS.Core.Dependency.ILifetimeScopeDependency
    {
        //.Net泛型不支持有参数的构造函数

        private AuthorityManager m_authority;
        public AuthorityManager Authority
        {
            get
            {
                return m_authority;
            }
            set
            {
                m_authority = value;
            }
        }

        private LazyConstruct<ProjectManager> m_project;
        public ProjectManager Project
        {
            get
            {
                return m_project.Get();
            }
        }

        private LazyConstruct<ProjectSeriesManager> m_projectSeries;
        public ProjectSeriesManager ProjectSeries
        {
            get
            {
                return m_projectSeries.Get();
            }
        }

        private LazyConstruct<ProjectActivityManager> m_projectActivity;
        public ProjectActivityManager ProjectActivity
        {
            get
            {
                return m_projectActivity.Get();
            }
        }

        private LazyConstruct<DocumentManager> m_document;
        public DocumentManager Document
        {
            get
            {
                return m_document.Get();
            }
        }

        private LazyConstruct<PermissionManager> m_permission;
        public PermissionManager Permission
        {
            get
            {
                return m_permission.Get();
            }
        }

        private LazyConstruct<TeamMemberManager> m_teamManaget;
        public TeamMemberManager TeamMember
        {
            get
            {
                return m_teamManaget.Get();
            }
        }

        private LazyConstruct<TeamAdminManager> m_teamAdmin;
        public TeamAdminManager TeamAdmin
        {
            get
            {
                return m_teamAdmin.Get();
            }
        }

        private LazyConstruct<UserGroupManager> m_userGroup;
        public UserGroupManager UserGroup
        {
            get
            {
                return m_userGroup.Get();
            }
        }

        private LazyConstruct<UserGroupMapManager> m_userGroupMap;
        public UserGroupMapManager UserGroupMap
        {
            get
            {
                return m_userGroupMap.Get();
            }
        }

        private LazyConstruct<TaskManager> m_task;
        public TaskManager Task
        {
            get
            {
                return m_task.Get();
            }
        }

        private LazyConstruct<TaskGroupManager> m_taskGroup;
        public TaskGroupManager TaskGroup
        {
            get
            {
                return m_taskGroup.Get();
            }
        }

        private LazyConstruct<TemplateManager> m_template;
        public TemplateManager Template
        {
            get
            {
                return m_template.Get();
            }
        }

        private LazyConstruct<ModelManager> m_model;
        public ModelManager Model
        {
            get
            {
                return m_model.Get();
            }
        }

        private LazyConstruct<ContactManager> m_contact;
        public ContactManager Contact
        {
            get
            {
                return m_contact.Get();
            }
        }

        private LazyConstruct<RemindSettingManager> m_remindSetting;
        public RemindSettingManager RemindSetting
        {
            get
            {
                return m_remindSetting.Get();
            }
        }

        private LazyConstruct<ReminderManager> m_reminder;
        public ReminderManager Reminder
        {
            get
            {
                return m_reminder.Get();
            }
        }

        private LazyConstruct<MonitorManager> m_monitor;
        public MonitorManager Monitor
        {
            get
            {
                return m_monitor.Get();
            }
        }

        private LazyConstruct<AgendaManager> m_agenda;
        public AgendaManager Agenda
        {
            get
            {
                return m_agenda.Get();
            }
        }

        private LazyConstruct<DMSManager> m_dms;
        public DMSManager DMS
        {
            get
            {
                return m_dms.Get();
            }
        }

        private LazyConstruct<DMSFileManager> m_dmsFile;
        public DMSFileManager DMSFile
        {
            get
            {
                return m_dmsFile.Get();
            }
        }

        private LazyConstruct<DMSFileSeriesManager> m_dmsFileSeries;
        public DMSFileSeriesManager DMSFileSeries
        {
            get
            {
                return m_dmsFileSeries.Get();
            }
        }

        private LazyConstruct<DMSFolderManager> m_dmsFolder;
        public DMSFolderManager DMSFolder
        {
            get
            {
                return m_dmsFolder.Get();
            }
        }

        private LazyConstruct<DMSTaskManager> m_dmsTask;
        public DMSTaskManager DMSTask
        {
            get
            {
                return m_dmsTask.Get();
            }
        }

        private LazyConstruct<DMSDurationManagementPlatformManager> m_dmsDuration;
        public DMSDurationManagementPlatformManager DMSDuration
        {
            get
            {
                return m_dmsDuration.Get();
            }
        }

        private LazyConstruct<DMSProjectLogManager> m_dmsProjectLog;
        public DMSProjectLogManager DMSProjectLog
        {
            get
            {
                return m_dmsProjectLog.Get();
            }
        }

        private LazyConstruct<DMSFileSeriesTemplateManager> m_dmsFileSeriesTemplate;
        public DMSFileSeriesTemplateManager DMSFileSeriesTemplate
        {
            get
            {
                return m_dmsFileSeriesTemplate.Get();
            }
        }

        private LazyConstruct<DatasetManager> m_dataset;
        public DatasetManager Dataset
        {
            get
            {
                return m_dataset.Get();
            }
        }

        private LazyConstruct<FileManager> m_file;
        public FileManager File
        {
            get
            {
                return m_file.Get();
            }
        }

        private LazyConstruct<ImageManager> m_image;
        public ImageManager Image
        {
            get
            {
                return m_image.Get();
            }
        }

        private LazyConstruct<PaymentHistoryManager> m_paymentHistory;
        public PaymentHistoryManager PaymentHistory
        {
            get
            {
                return m_paymentHistory.Get();
            }
        }

        private LazyConstruct<InvestmentManager> m_investment;
        public InvestmentManager Investment
        {
            get
            {
                return m_investment.Get();
            }
        }

        private LazyConstruct<SuperUserManager> m_superUser;
        public SuperUserManager SuperUser
        {
            get
            {
                return m_superUser.Get();
            }
        }

        private LazyConstruct<IssueManager> m_issue;
        public IssueManager Issue
        {
            get
            {
                return m_issue.Get();
            }
        }

        private LazyConstruct<IssueActivityManager> m_issueActivity;
        public IssueActivityManager IssueActivity
        {
            get
            {
                return m_issueActivity.Get();
            }
        }

        private LazyConstruct<IssueConnectionTasksManager> m_issueConnectionTasks;
        public IssueConnectionTasksManager IssueConnectionTasks
        {
            get
            {
                return m_issueConnectionTasks.Get();
            }
        }

        private LazyConstruct<DownloadFileAuthorityManager> m_downloadFileAuthority;
        public DownloadFileAuthorityManager DownloadFileAuthority
        {
            get
            {
                return m_downloadFileAuthority.Get();
            }
        }

        private LazyConstruct<PrepaymentSetManager> m_prepaymentSet;
        public PrepaymentSetManager PrepaymentSet
        {
            get
            {
                return m_prepaymentSet.Get();
            }
        }

        private LazyConstruct<PrepaymentManager> m_prepayment;
        public PrepaymentManager Prepayment
        {
            get
            {
                return m_prepayment.Get();
            }
        }

        private LazyConstruct<MessageRemindingManager> m_messageReminding;
        public MessageRemindingManager MessageReminding
        {
            get
            {
                return m_messageReminding.Get();
            }
        }

        private LazyConstruct<EmailContextManager> m_emailContext;
        public EmailContextManager EmailContext
        {
            get
            {
                return m_emailContext.Get();
            }
        }

        private LazyConstruct<EmailFromToManager> m_emailFromTo;
        public EmailFromToManager EmailFromTo
        {
            get
            {
                return m_emailFromTo.Get();
            }
        }

        private LazyConstruct<NegativeNewsManager> m_negativeNews;
        public NegativeNewsManager NegativeNews
        {
            get
            {
                return m_negativeNews.Get();
            }
        }

        private LazyConstruct<NewsManager> m_News;
        public NewsManager News
        {
            get
            {
                return m_News.Get();
            }
        }

        private LazyConstruct<AssetDefaultSetManager> m_assetDefaultSet;
        public AssetDefaultSetManager AssetDefaultSet
        {
            get
            {
                return m_assetDefaultSet.Get();
            }
        }

        private LazyConstruct<AssetDefaultManager> m_assetDefault;
        public AssetDefaultManager AssetDefault
        {
            get
            {
                return m_assetDefault.Get();
            }
        }

        private LazyConstruct<InterestRateAdjustmentSetManager> m_interestRateAdjustmentSet;
        public InterestRateAdjustmentSetManager InterestRateAdjustmentSet
        {
            get
            {
                return m_interestRateAdjustmentSet.Get();
            }
        }

        private LazyConstruct<InterestRateAdjustmentManager> m_interestRateAdjustment;
        public InterestRateAdjustmentManager InterestRateAdjustment
        {
            get
            {
                return m_interestRateAdjustment.Get();
            }
        }

        private LazyConstruct<MetaTaskManager> m_metaTask;
        public MetaTaskManager MetaTask
        {
            get
            {
                return m_metaTask.Get();
            }
        }

        private LazyConstruct<TimeOriginManager> m_timeOrigin;
        public TimeOriginManager TimeOrigin
        {
            get
            {
                return m_timeOrigin.Get();
            }
        }

        private LazyConstruct<TimeOriginCustomInputManager> m_timeOriginCustomInput;
        public TimeOriginCustomInputManager TimeOriginCustomInput
        {
            get
            {
                return m_timeOriginCustomInput.Get();
            }
        }

        private LazyConstruct<TimeOriginLoopManager> m_timeOriginLoop;
        public TimeOriginLoopManager TimeOriginLoop
        {
            get
            {
                return m_timeOriginLoop.Get();
            }
        }

        private LazyConstruct<TimeOriginMetaTaskManager> m_timeOriginMetaTask;
        public TimeOriginMetaTaskManager TimeOriginMetaTask
        {
            get
            {
                return m_timeOriginMetaTask.Get();
            }
        }

        private LazyConstruct<TimeRuleManager> m_timeRule;
        public TimeRuleManager TimeRule
        {
            get
            {
                return m_timeRule.Get();
            }
        }

        private LazyConstruct<TimeRuleConditionShiftManager> m_timeRuleConditionShift;
        public TimeRuleConditionShiftManager TimeRuleConditionShift
        {
            get
            {
                return m_timeRuleConditionShift.Get();
            }
        }

        private LazyConstruct<TimeRulePeriodSequenceManager> m_timeRulePeriodSequence;
        public TimeRulePeriodSequenceManager TimeRulePeriodSequence
        {
            get
            {
                return m_timeRulePeriodSequence.Get();
            }
        }

        private LazyConstruct<TimeRuleShiftManager> m_timeRuleShift;
        public TimeRuleShiftManager TimeRuleShift
        {
            get
            {
                return m_timeRuleShift.Get();
            }
        }

        private LazyConstruct<TimeSeriesManager> m_timeSeries;
        public TimeSeriesManager TimeSeries
        {
            get
            {
                return m_timeSeries.Get();
            }
        }

        private LazyConstruct<AssetCashflowVariableManager> m_assetCashflowVariable;
        public AssetCashflowVariableManager AssetCashflowVariable
        {
            get
            {
                return m_assetCashflowVariable.Get();
            }
        }

        private LazyConstruct<CashflowVariableManager> m_cashflowVariable;
        public CashflowVariableManager CashflowVariable
        {
            get
            {
                return m_cashflowVariable.Get();
            }
        }

        private LazyConstruct<TimeOriginTaskSelfTimeManager> m_timeOriginTaskSelfTime;
        public TimeOriginTaskSelfTimeManager TimeOriginTaskSelfTime
        {
            get
            {
                return m_timeOriginTaskSelfTime.Get();
            }
        }

        private LazyConstruct<AccountManager> m_bankAccount;
        public AccountManager BankAccount
        {
            get
            {
                return m_bankAccount.Get();
            }
        }

        private LazyConstruct<UserActionHabitsManager> m_userActionHabits;
        public UserActionHabitsManager UserActionHabits
        {
            get
            {
                return m_userActionHabits.Get();
            }
        }

        private LazyConstruct<TransactionManager> m_transaction;
        public TransactionManager Transaction
        {
            get
            {
                return m_transaction.Get();
            }
        }

        private LazyConstruct<UserVisitLogManager> m_userVisitLog;
        public UserVisitLogManager UserVisitLog
        {
            get
            {
                return m_userVisitLog.Get();
            }
        }

        private LazyConstruct<OverrideSingleAssetInterestManager> m_oerrideSingleAssetInterestManager;
        public OverrideSingleAssetInterestManager OverrideSingleAssetInterest
        {
            get
            {
                return m_oerrideSingleAssetInterestManager.Get();
            }
        }

        private LazyConstruct<OverrideSingleAssetPrincipalManager> m_oerrideSingleAssetPrincipalManager;
        public OverrideSingleAssetPrincipalManager OverrideSingleAssetPrincipal
        {
            get
            {
                return m_oerrideSingleAssetPrincipalManager.Get();
            }
        }

        private LazyConstruct<OverrideSingleAssetPrincipalBalanceManager> m_oerrideSingleAssetPrincipalBalanceManager;
        public OverrideSingleAssetPrincipalBalanceManager OverrideSingleAssetPrincipalBalance
        {
            get
            {
                return m_oerrideSingleAssetPrincipalBalanceManager.Get();
            }
        }

        private LazyConstruct<TaskPeriodManager> m_taskPeriod;
        public TaskPeriodManager TaskPeriod
        {
            get
            {
                return m_taskPeriod.Get();
            }
        }

        private LazyConstruct<CacheAcfAssetReceiveManager> m_cacheAcfAssetReceive;
        public CacheAcfAssetReceiveManager CacheAcfAssetReceive
        {
            get
            {
                return m_cacheAcfAssetReceive.Get();
            }
        }

        private LazyConstruct<CacheAcfReceiveManager> m_cacheAcfReceive;
        public CacheAcfReceiveManager CacheAcfReceive
        {
            get
            {
                return m_cacheAcfReceive.Get();
            }
        }

        private LazyConstruct<DealModelSettingManager> m_dealModelSetting;
        public DealModelSettingManager DealModelSetting
        {
            get
            {
                return m_dealModelSetting.Get();
            }
        }

        private LazyConstruct<EditModelCSVManager> m_editModelCsv;
        public EditModelCSVManager EditModelCsv
        {
            get
            {
                return m_editModelCsv.Get();
            }
        }

    }
}
