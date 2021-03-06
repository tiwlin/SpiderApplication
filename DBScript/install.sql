USE [master]
GO
/****** Object:  Database [TuanGou]    Script Date: 07/09/2014 16:37:55 ******/
CREATE DATABASE [TuanGou] ON  PRIMARY 
( NAME = N'TuanGou', FILENAME = N'D:\Database\TuanGou.mdf' , SIZE = 102400KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10240KB )
 LOG ON 
( NAME = N'TuanGou_log', FILENAME = N'D:\Database\TuanGou_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [TuanGou] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TuanGou].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TuanGou] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [TuanGou] SET ANSI_NULLS OFF
GO
ALTER DATABASE [TuanGou] SET ANSI_PADDING OFF
GO
ALTER DATABASE [TuanGou] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [TuanGou] SET ARITHABORT OFF
GO
ALTER DATABASE [TuanGou] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [TuanGou] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [TuanGou] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [TuanGou] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [TuanGou] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [TuanGou] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [TuanGou] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [TuanGou] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [TuanGou] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [TuanGou] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [TuanGou] SET  DISABLE_BROKER
GO
ALTER DATABASE [TuanGou] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [TuanGou] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [TuanGou] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [TuanGou] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [TuanGou] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [TuanGou] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [TuanGou] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [TuanGou] SET  READ_WRITE
GO
ALTER DATABASE [TuanGou] SET RECOVERY FULL
GO
ALTER DATABASE [TuanGou] SET  MULTI_USER
GO
ALTER DATABASE [TuanGou] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [TuanGou] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'TuanGou', N'ON'
GO
USE [TuanGou]
GO
/****** Object:  Table [dbo].[Shop]    Script Date: 07/09/2014 16:37:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Shop](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Category] [varchar](50) NOT NULL,
	[Region] [varchar](50) NOT NULL,
	[ShopID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Address] [varchar](max) NULL,
	[Range] [varchar](100) NULL,
	[RangeID] [int] NULL,
	[DisID] [int] NULL,
	[DisName] [varchar](50) NULL,
	[DPshopID] [int] NOT NULL,
	[MapUrl] [varchar](100) NULL,
	[TrafficInfo] [varchar](max) NULL,
	[Phone] [varchar](50) NOT NULL,
	[Latlng] [varchar](50) NULL,
	[City] [int] NOT NULL,
	[Url] [varchar](100) NULL,
	[CityName] [varchar](50) NULL,
	[Status] [bit] NULL,
	[SubwayName] [varchar](50) NULL,
	[SubwayDis] [varchar](100) NULL,
	[SubwaySlug] [varchar](50) NULL,
	[AppointmentDay] [varchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[TimeStamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_Shop] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Region]    Script Date: 07/09/2014 16:37:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Region](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RegionID] [int] NULL,
	[ParentRegion] [varchar](50) NULL,
	[Name] [varchar](50) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[TimeStamp] [timestamp] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InitUrls]    Script Date: 07/09/2014 16:37:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InitUrls](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NULL,
	[CategoryCode] [varchar](50) NOT NULL,
	[RegionID] [int] NULL,
	[RegionCode] [varchar](50) NOT NULL,
	[Url] [varchar](max) NOT NULL,
	[Status] [bit] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[TimeStamp] [timestamp] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Category]    Script Date: 07/09/2014 16:37:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Category](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NULL,
	[ParentCategory] [varchar](50) NULL,
	[Name] [varchar](50) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[TimeStamp] [timestamp] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_Region_CreateTime]    Script Date: 07/09/2014 16:37:56 ******/
ALTER TABLE [dbo].[Region] ADD  CONSTRAINT [DF_Region_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
/****** Object:  Default [DF_Region_UpdateTime]    Script Date: 07/09/2014 16:37:56 ******/
ALTER TABLE [dbo].[Region] ADD  CONSTRAINT [DF_Region_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO
/****** Object:  Default [DF_InitUrls_Status]    Script Date: 07/09/2014 16:37:56 ******/
ALTER TABLE [dbo].[InitUrls] ADD  CONSTRAINT [DF_InitUrls_Status]  DEFAULT ((0)) FOR [Status]
GO
/****** Object:  Default [DF_InitUrls_CreateTime]    Script Date: 07/09/2014 16:37:56 ******/
ALTER TABLE [dbo].[InitUrls] ADD  CONSTRAINT [DF_InitUrls_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
/****** Object:  Default [DF_InitUrls_UpdateTime]    Script Date: 07/09/2014 16:37:56 ******/
ALTER TABLE [dbo].[InitUrls] ADD  CONSTRAINT [DF_InitUrls_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO
/****** Object:  Default [DF_Category_CreateTime]    Script Date: 07/09/2014 16:37:56 ******/
ALTER TABLE [dbo].[Category] ADD  CONSTRAINT [DF_Category_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
/****** Object:  Default [DF_Category_UpdateTime]    Script Date: 07/09/2014 16:37:56 ******/
ALTER TABLE [dbo].[Category] ADD  CONSTRAINT [DF_Category_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO
