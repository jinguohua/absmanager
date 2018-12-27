-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'MetaTask'))
BEGIN
  CREATE TABLE [dbo].[MetaTask](
    [meta_task_id] [int] IDENTITY(1,1) NOT NULL,
    [meta_task_guid] [varchar](100) NOT NULL,
    [project_id] [int]  NOT NULL,
    [name] [varchar] (100) NOT NULL,
    [start_time_series_id] [int]  NOT NULL,
    [end_time_series_id] [int]  NOT NULL,
    [pre_meta_task_ids] [varchar] (1024),
    [detail] [varchar] (max),
    [target] [varchar] (max),
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
    [extension_type] [varchar](100),
 CONSTRAINT [PK_MetaTask] PRIMARY KEY CLUSTERED
 (
 [meta_task_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]  TEXTIMAGE_ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO


-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeOriginMetaTask'))
BEGIN
  CREATE TABLE [dbo].[TimeOriginMetaTask](
    [time_origin_meta_task_id] [int] IDENTITY(1,1) NOT NULL,
    [time_origin_meta_task_guid] [varchar](100) NOT NULL,
    [time_series_id] [int]  NOT NULL,
    [meta_task_id] [int]  NOT NULL,
    [meta_task_time_type_id] [int]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeOriginMetaTask] PRIMARY KEY CLUSTERED
 (
 [time_origin_meta_task_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO


-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeOriginLoop'))
BEGIN
  CREATE TABLE [dbo].[TimeOriginLoop](
    [time_origin_loop_id] [int] IDENTITY(1,1) NOT NULL,
    [time_origin_loop_guid] [varchar](100) NOT NULL,
    [time_series_id] [int]  NOT NULL,
    [loop_begin] [datetime]  NOT NULL,
    [loop_end] [datetime]  NOT NULL,
    [loop_interval] [int]  NOT NULL,
    [time_rule_unit_type_id] [int]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeOriginLoop] PRIMARY KEY CLUSTERED
 (
 [time_origin_loop_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeOriginCustomInput'))
BEGIN
  CREATE TABLE [dbo].[TimeOriginCustomInput](
    [time_origin_custom_input_id] [int] IDENTITY(1,1) NOT NULL,
    [time_origin_custom_input_guid] [varchar](100) NOT NULL,
    [time_series_id] [int]  NOT NULL,
    [custom_time_series] [varchar] (max) NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeOriginCustomInput] PRIMARY KEY CLUSTERED
 (
 [time_origin_custom_input_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]  TEXTIMAGE_ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeOrigin'))
BEGIN
  CREATE TABLE [dbo].[TimeOrigin](
    [time_origin_id] [int] IDENTITY(1,1) NOT NULL,
    [time_origin_guid] [varchar](100) NOT NULL,
    [time_series_id] [int]  NOT NULL,
    [time_origin_type_id] [int]  NOT NULL,
    [time_origin_instance_id] [int]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeOrigin] PRIMARY KEY CLUSTERED
 (
 [time_origin_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeSeries'))
BEGIN
  CREATE TABLE [dbo].[TimeSeries](
    [time_series_id] [int] IDENTITY(1,1) NOT NULL,
    [time_series_guid] [varchar](100) NOT NULL,
    [name] [varchar] (100) NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeSeries] PRIMARY KEY CLUSTERED
 (
 [time_series_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeRule'))
BEGIN
  CREATE TABLE [dbo].[TimeRule](
    [time_rule_id] [int] IDENTITY(1,1) NOT NULL,
    [time_rule_guid] [varchar](100) NOT NULL,
    [time_series_id] [int]  NOT NULL,
    [time_rule_order] [int]  NOT NULL,
    [time_rule_type_id] [int]  NOT NULL,
    [time_rule_instance_id] [int]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeRule] PRIMARY KEY CLUSTERED
 (
 [time_rule_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeRuleShift'))
BEGIN
  CREATE TABLE [dbo].[TimeRuleShift](
    [time_rule_shift_id] [int] IDENTITY(1,1) NOT NULL,
    [time_rule_shift_guid] [varchar](100) NOT NULL,
    [time_series_id] [int]  NOT NULL,
    [time_interval] [int]  NOT NULL,
    [time_rule_unit_type_id] [int]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeRuleShift] PRIMARY KEY CLUSTERED
 (
 [time_rule_shift_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeRulePeriodSequence'))
BEGIN
  CREATE TABLE [dbo].[TimeRulePeriodSequence](
    [time_rule_period_sequence_id] [int] IDENTITY(1,1) NOT NULL,
    [time_rule_period_sequence_guid] [varchar](100) NOT NULL,
    [time_series_id] [int]  NOT NULL,
    [time_rule_period_type_id] [int]  NOT NULL,
    [time_rule_period_sequence] [int]  NOT NULL,
    [time_rule_unit_type_id] [int]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeRulePeriodSequence] PRIMARY KEY CLUSTERED
 (
 [time_rule_period_sequence_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------


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
  AND  TABLE_NAME = 'TimeRuleConditionShift'))
BEGIN
  CREATE TABLE [dbo].[TimeRuleConditionShift](
    [time_rule_condition_shift_id] [int] IDENTITY(1,1) NOT NULL,
    [time_rule_condition_shift_guid] [varchar](100) NOT NULL,
    [time_series_id] [int]  NOT NULL,
    [time_rule_condition_date_type_id] [int]  NOT NULL,
    [time_interval] [int]  NOT NULL,
    [time_rule_unit_type_id] [int]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_TimeRuleConditionShift] PRIMARY KEY CLUSTERED
 (
 [time_rule_condition_shift_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------

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
  AND  TABLE_NAME = 'MetaTaskTimeType'))
BEGIN
CREATE TABLE [dbo].[MetaTaskTimeType](
	[meta_task_time_type_id] [int] NOT NULL,
	[meta_task_time_type_name] [varchar](100) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------

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
  AND  TABLE_NAME = 'TimeOriginType'))
BEGIN
CREATE TABLE [dbo].[TimeOriginType](
	[time_origin_type_id] [int] NOT NULL,
	[time_origin_type_name] [varchar](100) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------

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
  AND  TABLE_NAME = 'TimeRuleDateType'))
BEGIN
CREATE TABLE [dbo].[TimeRuleDateType](
	[time_rule_date_type_id] [int] NOT NULL,
	[time_rule_date_type_name] [varchar](100) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------

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
  AND  TABLE_NAME = 'TimeRulePeriodType'))
BEGIN
CREATE TABLE [dbo].[TimeRulePeriodType](
	[time_rule_period_type_id] [int] NOT NULL,
	[time_rule_period_type_name] [varchar](100) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------

USE [ABSManagement]
GO
SET ANSI_NULLS ON
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
  AND  TABLE_NAME = 'TimeRuleUnitType'))
BEGIN
CREATE TABLE [dbo].[TimeRuleUnitType](
	[time_rule_unit_type_id] [int] NOT NULL,
	[time_rule_unit_type_name] [varchar](100) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO

-------------------------------------------------------------------------------------------------------------
----枚举表里的数据 ！！！！请先生成上面的数据库表，然后再执行下面的插入语句！！！！
DELETE FROM [ABSManagement].[dbo].[TimeRuleDateType]
INSERT [ABSManagement].[dbo].[TimeRuleDateType] ([time_rule_date_type_id], [time_rule_date_type_name]) VALUES (1, N'Day')
INSERT [ABSManagement].[dbo].[TimeRuleDateType] ([time_rule_date_type_id], [time_rule_date_type_name]) VALUES (2, N'TradingDay')
INSERT [ABSManagement].[dbo].[TimeRuleDateType] ([time_rule_date_type_id], [time_rule_date_type_name]) VALUES (3, N'WorkingDay')

DELETE FROM [ABSManagement].[dbo].[TimeRulePeriodType]
INSERT [ABSManagement].[dbo].[TimeRulePeriodType] ([time_rule_period_type_id], [time_rule_period_type_name]) VALUES (1, N'Year')
INSERT [ABSManagement].[dbo].[TimeRulePeriodType] ([time_rule_period_type_id], [time_rule_period_type_name]) VALUES (2, N'Month')
INSERT [ABSManagement].[dbo].[TimeRulePeriodType] ([time_rule_period_type_id], [time_rule_period_type_name]) VALUES (3, N'Week')

DELETE FROM [ABSManagement].[dbo].[TimeRuleUnitType]
INSERT [ABSManagement].[dbo].[TimeRuleUnitType] ([time_rule_unit_type_id], [time_rule_unit_type_name]) VALUES (1, N'Year')
INSERT [ABSManagement].[dbo].[TimeRuleUnitType] ([time_rule_unit_type_id], [time_rule_unit_type_name]) VALUES (2, N'Month')
INSERT [ABSManagement].[dbo].[TimeRuleUnitType] ([time_rule_unit_type_id], [time_rule_unit_type_name]) VALUES (3, N'Day')
INSERT [ABSManagement].[dbo].[TimeRuleUnitType] ([time_rule_unit_type_id], [time_rule_unit_type_name]) VALUES (4, N'TradingDay')
INSERT [ABSManagement].[dbo].[TimeRuleUnitType] ([time_rule_unit_type_id], [time_rule_unit_type_name]) VALUES (5, N'WorkingDay')

DELETE FROM [ABSManagement].[dbo].[MetaTaskTimeType]
INSERT [ABSManagement].[dbo].[MetaTaskTimeType] ([meta_task_time_type_id], [meta_task_time_type_name]) VALUES (1, N'StartTime')
INSERT [ABSManagement].[dbo].[MetaTaskTimeType] ([meta_task_time_type_id], [meta_task_time_type_name]) VALUES (2, N'EndTime')

DELETE FROM [ABSManagement].[dbo].[TimeOriginType]
INSERT [ABSManagement].[dbo].[TimeOriginType] ([time_origin_type_id], [time_origin_type_name]) VALUES (1, N'CustomInput')
INSERT [ABSManagement].[dbo].[TimeOriginType] ([time_origin_type_id], [time_origin_type_name]) VALUES (2, N'Loop')
INSERT [ABSManagement].[dbo].[TimeOriginType] ([time_origin_type_id], [time_origin_type_name]) VALUES (3, N'MetaTask')
-------------------------------------------------------------------------------------------------------------
