--2017/6/27
	--创建表[dbo].[Prepayment]
	/****** Object:  Table [dbo].[Prepayment]    Script Date: 2017/6/27 14:46:58 ******/

	
USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'Prepayment'))
	BEGIN
	  CREATE TABLE [dbo].[Prepayment](
	  	  [prepayment_id] [int] IDENTITY(1,1) NOT NULL,
		  [prepayment_guid] [varchar](100) NOT NULL,
		  [prepayment_set_id] [int]  NOT NULL,
		  [asset_id] [int]  NOT NULL,
		  [money] [float]  NOT NULL,
		  [prepay_date] [datetime]  NOT NULL,
		  [origin_date] [datetime]  NOT NULL,
		  [create_time] [datetime] NOT NULL,
		  [create_user_name] [varchar](100) NOT NULL,
		  [last_modify_time] [datetime] NOT NULL,
		  [last_modify_user_name] [varchar](100) NOT NULL,
		  [record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_Prepayment] PRIMARY KEY CLUSTERED
	 (
		 [prepayment_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO

	--创建表[dbo].[PrepaymentSet]
	/****** Object:  Table [dbo].[PrepaymentSet]    Script Date: 2017/6/27 14:47:08 ******/
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'PrepaymentSet'))
	BEGIN
	  CREATE TABLE [dbo].[PrepaymentSet](
	    [prepayment_set_id] [int] IDENTITY(1,1) NOT NULL,
	    [prepayment_set_guid] [varchar](100) NOT NULL,
	    [project_id] [int]  NOT NULL,
	    [payment_date] [datetime]  NOT NULL,
	    [name] [varchar] (100) NOT NULL,
	    [create_time] [datetime] NOT NULL,
	    [create_user_name] [varchar](100) NOT NULL,
	    [last_modify_time] [datetime] NOT NULL,
	    [last_modify_user_name] [varchar](100) NOT NULL,
	    [record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_PrepaymentSet] PRIMARY KEY CLUSTERED
	 (
	 [prepayment_set_id] ASC
		  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 	ALLOW_ROW_LOCKS = ON, 	ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO


--2017/7/3
	--[Tasks]增加字段 meta_task_id
	IF (NOT EXISTS (SELECT NAME
	 FROM SYSCOLUMNS 
	WHERE ID = OBJECT_ID('[ABSManagement].[dbo].[Tasks]')
	 AND NAME = 'meta_task_id'))
	BEGIN
	ALTER TABLE [ABSManagement].[dbo].[Tasks] ADD [meta_task_id] int
	END
	GO


--2017/7/6
	--数据库[ABSManagement]中增加【根据自定义规则生成工作】的一系列的表
	--#cnabsFileReference#：../sql script/根据自定义规则生成工作的数据库表的sql语句.txt

--2017/7/12
	--[Project]增加字段 email
	IF (NOT EXISTS (SELECT NAME
	 FROM SYSCOLUMNS 
	WHERE ID = OBJECT_ID('[ABSManagement].[dbo].[ProjectSeries]')
	 AND NAME = 'email'))
	BEGIN
	ALTER TABLE [ABSManagement].[dbo].[ProjectSeries] ADD [email] varchar(100)
	END
	GO


--2017/7/20
	--创建表[NegativeNews] by mxdong
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'NegativeNews'))
	BEGIN
	  CREATE TABLE [dbo].[NegativeNews](
		[negative_news_id] [int] IDENTITY(1,1) NOT NULL,
		[negative_news_guid] [varchar](100) NOT NULL,
		[project_id] [int]  NOT NULL,
		[username] [varchar] (100) NOT NULL,
		[subscribe_time] [datetime]  NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_NegativeNews] PRIMARY KEY CLUSTERED
	 (
	 [negative_news_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO

	--创建表[dbo].[MessageReminding],
	USE [ABSManagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'MessageReminding'))
	BEGIN
	CREATE TABLE [dbo].[MessageReminding](
		[message_reminding_id] [int] IDENTITY(1,1) NOT NULL,
		[message_reminding_guid] [varchar](100) NOT NULL,
		[uid] [varchar](100) NOT NULL,
		[userid] [varchar](100) NOT NULL,
		[remind_time] [datetime] NOT NULL,
		[remark] [varchar](500) NULL,
		[type] [int] NOT NULL,
		[message_status] [int] NOT NULL,
		[message_time] [datetime] NULL,
		[message_content] [varchar](max) NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_MessageReminding] PRIMARY KEY CLUSTERED 
	(
		[message_reminding_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END
	GO
	SET ANSI_PADDING OFF
	GO

	
	--创建表[dbo].[AssetCashflowVariable],
	USE [absmanagement]
	GO

	/****** Object:  Table [dbo].[AssetCashflowVariable]    Script Date: 7/28/2017 11:30:51 AM ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO

    IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'AssetCashflowVariable'))
	BEGIN
	CREATE TABLE [dbo].[AssetCashflowVariable](
		[asset_cashflow_variable_id] [int] IDENTITY(1,1) NOT NULL,
		[asset_cashflow_variable_guid] [varchar](100) NOT NULL,
		[project_id] [int] NOT NULL,
		[payment_date] [datetime] NOT NULL,
		[interest_collection] [float] NOT NULL,
		[pricipal_collection] [float] NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_AssetCashflowVariable] PRIMARY KEY CLUSTERED 
	(
		[asset_cashflow_variable_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
    END
	GO

	SET ANSI_PADDING OFF
	GO
	
--2017/08/02 
	--创建表[dbo].[TeamAdmin],
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'TeamAdmin'))
	BEGIN
	  CREATE TABLE [dbo].[TeamAdmin](
		[team_admin_id] [int] IDENTITY(1,1) NOT NULL,
		[team_admin_guid] [varchar](100) NOT NULL,
		[project_id] [int]  NOT NULL,
		[user_name] [varchar] (100) NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_TeamAdmin] PRIMARY KEY CLUSTERED
	 (
	 [team_admin_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	--创建表[dbo].[TeamAdmin],
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'TimeOriginTaskSelfTime'))
	BEGIN
	  CREATE TABLE [dbo].[TimeOriginTaskSelfTime](
		[time_origin_task_self_time_id] [int] IDENTITY(1,1) NOT NULL,
		[time_origin_task_self_time_guid] [varchar](100) NOT NULL,
		[time_series_id] [int]  NOT NULL,
		[time_origin_time_series_guid] [varchar] (100) NOT NULL,
		[time_type] [int] NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_TimeOriginTaskSelfTime] PRIMARY KEY CLUSTERED
	 (
	 [time_origin_task_self_time_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO

	--创建表[dbo].[UserGroup]
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'UserGroup'))
	BEGIN
	  CREATE TABLE [dbo].[UserGroup](
		[user_group_id] [int] IDENTITY(1,1) NOT NULL,
		[user_group_guid] [varchar](100) NOT NULL,
		[name] [varchar] (100) NOT NULL,
		[owner] [varchar] (100) NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED
	 (
	 [user_group_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO

	--创建表[dbo].[UserGroupMap]
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'UserGroupMap'))
	BEGIN
	  CREATE TABLE [dbo].[UserGroupMap](
		[user_group_map_id] [int] IDENTITY(1,1) NOT NULL,
		[user_group_map_guid] [varchar](100) NOT NULL,
		[user_group_guid] [varchar] (100) NOT NULL,
		[user_name] [varchar] (100) NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_UserGroupMap] PRIMARY KEY CLUSTERED
	 (
	 [user_group_map_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	
	--创建表 DMSTask :by mxdong
	--time 2017/08/23
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMSTask'))
	BEGIN
	  CREATE TABLE [dbo].[DMSTask](
		[dms_task_id] [int] IDENTITY(1,1) NOT NULL,
		[dms_task_guid] [varchar](100) NOT NULL,
		[dms_id] [int]  NOT NULL,
		[short_code] [varchar] (100) NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_DMSTask] PRIMARY KEY CLUSTERED
	 (
	 [dms_task_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	--创建表 2017/10/16
	--本地版系统中没有Issue表
	USE [ABSManagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'Issue'))
	BEGIN
	CREATE TABLE [dbo].[Issue](
		[issue_id] [int] IDENTITY(1,1) NOT NULL,
		[issue_guid] [varchar](100) NOT NULL,
		[project_id] [int] NOT NULL,
		[issue_name] [varchar](100) NOT NULL,
		[description] [varchar](max) NOT NULL,
		[issue_emergency_level_id] [int] NOT NULL,
		[related_task_short_code] [varchar](max) NOT NULL,
		[issue_status_id] [int] NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	SET ANSI_PADDING OFF
	ALTER TABLE [dbo].[Issue] ADD [issue_allot_user] [varchar](100) NULL
	SET ANSI_PADDING ON
	ALTER TABLE [dbo].[Issue] ADD [last_modify_user_name] [varchar](100) NULL
	ALTER TABLE [dbo].[Issue] ADD [last_modify_time] [datetime] NULL
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	--[Issue]新增字段 [issue_allot_user] :by bli
	--time 2017/09/26
	IF (NOT EXISTS (SELECT NAME
	  FROM SYSCOLUMNS 
	  WHERE ID = OBJECT_ID('[ABSManagement].[dbo].[Issue]')
	  AND NAME = 'issue_allot_user'))
	BEGIN
	  ALTER TABLE [ABSManagement].[dbo].[Issue] ADD [issue_allot_user] varchar(100)
	END
	GO
	
	--新增表 [dbo].[IssueConnectionTasks] :by bli
	--time 2017/09/26
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'IssueConnectionTasks'))
	BEGIN
	  CREATE TABLE [dbo].[IssueConnectionTasks](
		[issue_connection_tasks_id] [int] IDENTITY(1,1) NOT NULL,
		[issue_connection_tasks_guid] [varchar](100) NOT NULL,
		[task_short_code] [varchar] (100) NOT NULL,
		[issue_id] [int]  NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_IssueConnectionTasks] PRIMARY KEY CLUSTERED
	 (
	 [issue_connection_tasks_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	--[Issue]新增字段 [last_modify_time] :by bli
	--time 2017/09/29
	IF (NOT EXISTS (SELECT NAME
	 FROM SYSCOLUMNS 
	WHERE ID = OBJECT_ID('[ABSManagement].[dbo].[Issue]')
	 AND NAME = 'last_modify_time'))
	BEGIN
	ALTER TABLE [ABSManagement].[dbo].[Issue] ADD [last_modify_time] datetime
	END
	GO
	
	--[Issue]新增字段 [last_modify_user_name] :by bli
	--time 2017/09/29
	IF (NOT EXISTS (SELECT NAME
	 FROM SYSCOLUMNS 
	WHERE ID = OBJECT_ID('[ABSManagement].[dbo].[Issue]')
	 AND NAME = 'last_modify_user_name'))
	BEGIN
	ALTER TABLE [ABSManagement].[dbo].[Issue] ADD [last_modify_user_name] varchar(100)
	END
	GO
	
	--[Issue]新增字段 [issue_allot_time] :by bli
	--time 2017/10/25
	IF (NOT EXISTS (SELECT NAME
	 FROM SYSCOLUMNS 
	WHERE ID = OBJECT_ID('[ABSManagement].[dbo].[Issue]')
	 AND NAME = 'issue_allot_time'))
	BEGIN
	ALTER TABLE [ABSManagement].[dbo].[Issue] ADD [issue_allot_time] datetime
	END
	GO
	
	--新增表 [dbo].[DMSDurationManagementPlatform] :by mxdong
	--time 2017/10/26
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMSDurationManagementPlatform'))
	BEGIN
	  CREATE TABLE [dbo].[DMSDurationManagementPlatform](
		[dms_duration_management_platform_id] [int] IDENTITY(1,1) NOT NULL,
		[dms_duration_management_platform_guid] [varchar](100) NOT NULL,
		[project_id] [int]  NOT NULL,
		[dms_id] [int]  NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_DMSDurationManagementPlatform] PRIMARY KEY CLUSTERED
	 (
	 [dms_duration_management_platform_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	
	--新增表 [dbo].[DMSProjectLog] :by mxdong
	--time 2017/10/26
	USE [ABSManagement]
	GO

	
	/****** Object:  Table [dbo].[DMSProjectLog]    Script Date: 2017/10/26 10:20:13 ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMSProjectLog'))
	BEGIN
	CREATE TABLE [dbo].[DMSProjectLog](
		[dms_project_log_id] [int] IDENTITY(1,1) NOT NULL,
		[project_guid] [varchar](100) NOT NULL,
		[fileseries_guid] [varchar](100) NULL,
		[time_stamp] [datetime] NULL,
		[time_stamp_user_name] [varchar](50) NULL,
		[comment] [varchar](max) NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END
	GO

	SET ANSI_PADDING OFF
	GO


	--新增表 [dbo].[DMSFileSeriesTemplate] :by cgzhou
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMSFileSeriesTemplate'))
	BEGIN
	  CREATE TABLE [dbo].[DMSFileSeriesTemplate](
		[dms_file_series_template_id] [int] IDENTITY(1,1) NOT NULL,
		[dms_file_series_template_guid] [varchar](100) NOT NULL,
		[dms_file_series_id] [int]  NOT NULL,
		[dms_file_series_template_type_id] [int]  NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_DMSFileSeriesTemplate] PRIMARY KEY CLUSTERED
	 (
	 [dms_file_series_template_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[DMS]    Script Date: 2017/10/30 10:22:26 ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMS'))
	BEGIN

	CREATE TABLE [dbo].[DMS](
		[dms_id] [int] IDENTITY(1,1) NOT NULL,
		[dms_guid] [varchar](100) NOT NULL,
		[project_id] [int] NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_DMS] PRIMARY KEY CLUSTERED 
	(
		[dms_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	END
	GO

	SET ANSI_PADDING OFF
	GO
	
	
	--新增表 [dbo].[DMSFile] :by bxjiang
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[DMSFile]    Script Date: 2017/10/30 14:12:14 ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO

	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMSFile'))
	BEGIN

	CREATE TABLE [dbo].[DMSFile](
		[dms_file_id] [int] IDENTITY(1,1) NOT NULL,
		[dms_file_guid] [varchar](100) NOT NULL,
		[dms_id] [int] NOT NULL,
		[file_id] [int] NOT NULL,
		[name] [nvarchar](50) NOT NULL,
		[version] [int] NOT NULL,
		[description] [varchar](max) NOT NULL,
		[size] [bigint] NOT NULL,
		[dms_file_series_id] [int] NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_DMSFile] PRIMARY KEY CLUSTERED 
	(
		[dms_file_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END
	GO

	SET ANSI_PADDING OFF
	GO
	
	--新增表 [dbo].[DMSFileSeries] :by bxjiang
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[DMSFileSeries]    Script Date: 2017/10/30 14:21:43 ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO
	
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMSFileSeries'))
	BEGIN

	CREATE TABLE [dbo].[DMSFileSeries](
		[dms_file_series_id] [int] IDENTITY(1,1) NOT NULL,
		[dms_file_series_guid] [varchar](100) NOT NULL,
		[dms_id] [int] NOT NULL,
		[dms_file_series_name] [varchar](100) NULL,
		[dms_folder_id] [int] NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_DMSFileSeries] PRIMARY KEY CLUSTERED 
	(
		[dms_file_series_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
    END
	GO

	SET ANSI_PADDING OFF
	GO
	
	
	--新增表 [dbo].[DMSFolder] :by bxjiang
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[DMSFolder]    Script Date: 2017/10/30 14:24:32 ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO
	
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMSFolder'))
	BEGIN

	CREATE TABLE [dbo].[DMSFolder](
		[dms_folder_id] [int] IDENTITY(1,1) NOT NULL,
		[dms_folder_guid] [varchar](100) NOT NULL,
		[dms_id] [int] NOT NULL,
		[name] [nvarchar](50) NOT NULL,
		[description] [varchar](max) NOT NULL,
		[parent_folder_id] [int] NULL,
		[dms_folder_type_id] [int] NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_DMSFolder] PRIMARY KEY CLUSTERED 
	(
		[dms_folder_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    END
	GO

	SET ANSI_PADDING OFF
	GO
	
	
	--新增表 [dbo].[DMSFolderType] :by bxjiang
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[DMSFolderType]    Script Date: 2017/10/30 14:26:44 ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO

	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DMSFolderType'))
	BEGIN
	
	CREATE TABLE [dbo].[DMSFolderType](
		[dms_folder_type_id] [int] NOT NULL,
		[dms_folder_type_name] [varchar](100) NOT NULL
	) ON [PRIMARY]
    END
	GO

	SET ANSI_PADDING OFF
	GO
	
	
	--新增表 [dbo].[File] :by bxjiang
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[File]    Script Date: 2017/10/30 14:59:06 ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	SET ANSI_PADDING ON
	GO
	
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'File'))
	BEGIN

	CREATE TABLE [dbo].[File](
		[file_id] [int] IDENTITY(1,1) NOT NULL,
		[file_guid] [varchar](100) NOT NULL,
		[name] [varchar](100) NOT NULL,
		[path] [varchar](max) NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    END
	GO

	SET ANSI_PADDING OFF
	GO

	
	--新增表 [dbo].[Image] :by cgzhou
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[Image]    Script Date: 11/8/2017 4:16:19 PM ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO

	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'Image'))
	BEGIN

	CREATE TABLE [dbo].[Image](
		[image_id] [int] IDENTITY(1,1) NOT NULL,
		[image_guid] [varchar](100) NOT NULL,
		[name] [nvarchar](500) NOT NULL,
		[path] [varchar](max) NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END
	GO
	
	
	--[DMSFile]修改字段类型 [name] :by bli
	alter table [ABSManagement].[dbo].[DMSFile] alter column name nvarchar(500) not null
	
	--[DMSFileSeries]修改字段类型 [dms_file_series_name] :by bli
	alter table [ABSManagement].[dbo].[DMSFileSeries] alter column dms_file_series_name nvarchar(500) not null
	
	--[DMSFolder]修改字段类型 [name] :by bli
	alter table [ABSManagement].[dbo].[DMSFolder] alter column name nvarchar(500) not null
	
	--[File]修改字段类型 [name] :by bli
	alter table [ABSManagement].[dbo].[File] alter column name nvarchar(500) not null
	
	--[Image]修改字段类型 [name] :by bli
	alter table [ABSManagement].[dbo].[Image] alter column name nvarchar(500) not null
	
	--[Document]修改字段类型 [name] :by bli
	alter table [ABSManagement].[dbo].[Document] alter column document_name nvarchar(500) not null



	--新增表 [dbo].[UserActionHabits] :by cgzhou
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'UserActionHabits'))
	BEGIN
	  CREATE TABLE [dbo].[UserActionHabits](
		[user_action_habits_id] [int] IDENTITY(1,1) NOT NULL,
		[user_action_habits_guid] [varchar](100) NOT NULL,
		[user_name] [varchar] (100) NOT NULL,
		[action_category_name] [varchar] (100) NOT NULL,
		[action_name] [varchar] (100) NOT NULL,
		[action_setting] [varchar] (100) NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_UserActionHabits] PRIMARY KEY CLUSTERED
	 (
	 [user_action_habits_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	
	--新增表 [dbo].[CashflowVariable] :by bli
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'CashflowVariable'))
	BEGIN
	  CREATE TABLE [dbo].[CashflowVariable](
		[cashflow_variable_id] [int] IDENTITY(1,1) NOT NULL,
		[cashflow_variable_guid] [varchar](100) NOT NULL,
		[project_id] [int]  NOT NULL,
		[payment_date] [datetime]  NOT NULL,
		[chinese_name] [varchar] (100) NOT NULL,
		[english_name] [varchar] (100) NOT NULL,
		[value] [float]  NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_CashflowVariable] PRIMARY KEY CLUSTERED
	 (
	 [cashflow_variable_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO

	--新增表 [dbo].[OverrideSingleAssetInterest] :by cgzhou
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[OverrideSingleAssetInterest]    Script Date: 11/17/2017 3:51:01 PM ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'OverrideSingleAssetInterest'))
	BEGIN

	CREATE TABLE [dbo].[OverrideSingleAssetInterest](
		[override_single_asset_interest_id] [int] IDENTITY(1,1) NOT NULL,
		[override_single_asset_interest_guid] [varchar](100) NOT NULL,
		[project_id] [int] NOT NULL,
		[payment_date] [datetime] NOT NULL,
		[asset_id] [int] NOT NULL,
		[interest] [float] NOT NULL,
		[comment] [varchar](max) NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_OverrideSingleAssetInterest] PRIMARY KEY CLUSTERED 
	(
		[override_single_asset_interest_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END
	GO





	--新增表 [dbo].[OverrideSingleAssetPrincipal] :by cgzhou
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[OverrideSingleAssetPrincipal]    Script Date: 11/17/2017 3:51:17 PM ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'OverrideSingleAssetPrincipal'))
	BEGIN

	CREATE TABLE [dbo].[OverrideSingleAssetPrincipal](
		[override_single_asset_principal_id] [int] IDENTITY(1,1) NOT NULL,
		[override_single_asset_principal_guid] [varchar](100) NOT NULL,
		[project_id] [int] NOT NULL,
		[payment_date] [datetime] NOT NULL,
		[asset_id] [int] NOT NULL,
		[principal] [float] NOT NULL,
		[comment] [varchar](max) NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_OverrideSingleAssetPrincipal] PRIMARY KEY CLUSTERED 
	(
		[override_single_asset_principal_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END
	GO




	--新增表 [dbo].[OverrideSingleAssetPrincipalBalance] :by cgzhou
	USE [ABSManagement]
	GO

	/****** Object:  Table [dbo].[OverrideSingleAssetPrincipalBalance]    Script Date: 11/17/2017 3:51:30 PM ******/
	SET ANSI_NULLS ON
	GO

	SET QUOTED_IDENTIFIER ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'OverrideSingleAssetPrincipalBalance'))
	BEGIN

	CREATE TABLE [dbo].[OverrideSingleAssetPrincipalBalance](
		[override_single_asset_principal_balance_id] [int] IDENTITY(1,1) NOT NULL,
		[override_single_asset_principal_balance_guid] [varchar](100) NOT NULL,
		[project_id] [int] NOT NULL,
		[payment_date] [datetime] NOT NULL,
		[asset_id] [int] NOT NULL,
		[principal_balance] [float] NOT NULL,
		[comment] [varchar](max) NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_OverrideSingleAssetPrincipalBalance] PRIMARY KEY CLUSTERED 
	(
		[override_single_asset_principal_balance_id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	END
	GO


	--新增表 [dbo].[TaskPeriod] :by bli
	USE [absmanagement]
	GO
	/****** Object:  Table [dbo].[TaskPeriod]    Script Date: 2017/11/23 14:01:00 ******/
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'TaskPeriod'))
	BEGIN
	  CREATE TABLE [dbo].[TaskPeriod](
		[task_period_id] [int] IDENTITY(1,1) NOT NULL,
		[task_period_guid] [varchar](100) NOT NULL,
		[project_id] [int]  NOT NULL,
		[short_code] [varchar] (100) NOT NULL,
		[payment_date] [datetime]  NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_TaskPeriod] PRIMARY KEY CLUSTERED
	 (
	 [task_period_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
	--新增表EditModelCSV by mxdong
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'EditModelCSV'))
	BEGIN
	  CREATE TABLE [dbo].[EditModelCSV](
		[edit_model_csv_id] [int] IDENTITY(1,1) NOT NULL,
		[edit_model_csv_guid] [varchar](100) NOT NULL,
		[type] [varchar] (100) NOT NULL,
		[title] [varchar] (100),
		[comment] [varchar] (max),
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_EditModelCSV] PRIMARY KEY CLUSTERED
	 (
	 [edit_model_csv_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]  TEXTIMAGE_ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	
		--修改表 [dbo].[DealModelSetting] :by dongmx
	IF (NOT EXISTS (SELECT NAME
	 FROM SYSCOLUMNS 
	WHERE ID = OBJECT_ID('[ABSManagement].[dbo].[EditModelCSV]')
	 AND NAME = 'project_guid'))
	BEGIN
		ALTER TABLE [ABSManagement].[dbo].[EditModelCSV] ADD [project_guid] [varchar](100) 
		ALTER TABLE [ABSManagement].[dbo].[EditModelCSV] ADD [asofdate] [varchar](8) 
	END
	GO


	IF (NOT EXISTS (SELECT NAME
	 FROM SYSCOLUMNS 
	WHERE ID = OBJECT_ID('[ABSManagement].[dbo].[AssetCashflowVariable]')
	 AND NAME = 'enable_override'))
	BEGIN
		ALTER TABLE [ABSManagement].[dbo].[AssetCashflowVariable] ADD [enable_override] bit not null default 0

	END
	GO
	
		--新增表 [dbo].[DealModelSetting] :by cgzhou
	
	USE [absmanagement]
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	SET ANSI_PADDING ON
	GO
	IF (NOT EXISTS (SELECT * 
	  FROM INFORMATION_SCHEMA.TABLES 
	  WHERE TABLE_SCHEMA = 'dbo' 
	  AND  TABLE_NAME = 'DealModelSetting'))
	BEGIN
	  CREATE TABLE [dbo].[DealModelSetting](
		[deal_model_setting_id] [int] IDENTITY(1,1) NOT NULL,
		[deal_model_setting_guid] [varchar](100) NOT NULL,
		[project_id] [int]  NOT NULL,
		[enable_predict_mode] [int]  NOT NULL,
		[create_time] [datetime] NOT NULL,
		[create_user_name] [varchar](100) NOT NULL,
		[last_modify_time] [datetime] NOT NULL,
		[last_modify_user_name] [varchar](100) NOT NULL,
		[record_status_id] [int] NOT NULL,
	 CONSTRAINT [PK_DealModelSetting] PRIMARY KEY CLUSTERED
	 (
	 [deal_model_setting_id] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
	END
	GO
	SET ANSI_PADDING OFF
	GO
	


	




