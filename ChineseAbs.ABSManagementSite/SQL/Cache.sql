--创建资产端结果缓存表：cgzhou 2017-12-01
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
  AND  TABLE_NAME = 'CacheAcfReceive'))
BEGIN
  CREATE TABLE [dbo].[CacheAcfReceive](
    [cache_acf_receive_id] [int] IDENTITY(1,1) NOT NULL,
    [cache_acf_receive_guid] [varchar](100) NOT NULL,
    [project_id] [int]  NOT NULL,
    [payment_day] [datetime]  NOT NULL,
    [principal] [float]  NOT NULL,
    [interest] [float]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_CacheAcfReceive] PRIMARY KEY CLUSTERED
 (
 [cache_acf_receive_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO


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
  AND  TABLE_NAME = 'CacheAcfAssetReceive'))
BEGIN
  CREATE TABLE [dbo].[CacheAcfAssetReceive](
    [cache_acf_asset_receive_id] [int] IDENTITY(1,1) NOT NULL,
    [cache_acf_asset_receive_guid] [varchar](100) NOT NULL,
    [project_id] [int]  NOT NULL,
    [payment_day] [datetime]  NOT NULL,
    [asset_id] [int]  NOT NULL,
    [principal] [float]  NOT NULL,
    [interest] [float]  NOT NULL,
    [perform] [float]  NOT NULL,
    [loss] [float]  NOT NULL,
    [defaulted] [float]  NOT NULL,
    [fee] [float]  NOT NULL,
    [create_time] [datetime] NOT NULL,
    [create_user_name] [varchar](100) NOT NULL,
    [last_modify_time] [datetime] NOT NULL,
    [last_modify_user_name] [varchar](100) NOT NULL,
    [record_status_id] [int] NOT NULL,
 CONSTRAINT [PK_CacheAcfAssetReceive] PRIMARY KEY CLUSTERED
 (
 [cache_acf_asset_receive_id] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] 
END
GO
SET ANSI_PADDING OFF
GO
