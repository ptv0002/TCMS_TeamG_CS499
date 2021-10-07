USE [master]
GO
/****** Object:  Database [TCMS_Database]    Script Date: 9/6/2021 12:33:37 AM ******/
CREATE DATABASE [TCMS_Database]
GO
USE [TCMS_Database]
GO
/****** Object:  Table [dbo].[AssignmentDetail]    Script Date: 9/6/2021 12:33:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignmentDetail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ShippingAssignmentlID] [int] NULL,
	[OrderInfoID] [int] NULL,
	[InShipping] [bit] NULL,
	[ArrivalConfirm] [bit] NULL,
	[ArrivalTime] [datetime] NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_AssignmentDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 9/6/2021 12:33:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Status] [bit] NULL,
	[Address] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](50) NULL,
	[Zip] [int] NULL,
	[ContactPerson] [varchar](max) NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 9/6/2021 12:33:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[ID] [nchar](10) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[MiddleName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Position] [varchar](50) NULL,
	[Status] [bit] NULL,
	[Address] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](50) NULL,
	[Zip] [int] NULL,
	[HomePhoneNum] [varchar](50) NULL,
	[CellPhone] [varchar](50) NULL,
	[PayRate] [float] NULL,
	[StartDate] [datetime] NULL,
	[Email] [varchar](50) NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MaintenanceDetail]    Script Date: 9/6/2021 12:33:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaintenanceDetail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MaintenanceInfoID] [int] NULL,
	[Service] [varchar](50) NULL,
	[EstimateCost] [float] NULL,
	[Notes] [varchar](max) NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_MaintenanceDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MaintenanceInfo]    Script Date: 9/6/2021 12:33:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaintenanceInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [nchar](10) NULL,
	[VehicleID] [nchar](10) NULL,
	[Datetime] [datetime] NULL,
	[Notes] [varchar](max) NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_MaintenanceInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderInfo]    Script Date: 9/6/2021 12:33:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceID] [int] NULL,
	[DestinationID] [int] NULL,
	[SourceAddress] [varchar](50) NULL,
	[DestinationAddresss] [varchar](50) NULL,
	[Status] [bit] NULL,
	[DocName] [varchar](100) NULL,
	[DocType] [varchar](100) NULL,
	[DocData] [varbinary](max) NULL,
	[SourcePay] [bit] NULL,
	[PayStatus] [bit] NULL,
	[TotalOrder] [float] NULL,
	[ShippingFee] [float] NULL,
	[EstimateArrivalTime] [datetime] NULL,
 CONSTRAINT [PK_ShippingInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingAssignment]    Script Date: 9/6/2021 12:33:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingAssignment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[VehicleID] [nchar](10) NULL,
	[EmployeeID] [nchar](10) NULL,
	[DepartureTime] [datetime] NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_ShippingAssignment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vehicle]    Script Date: 9/6/2021 12:33:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vehicle](
	[ID] [nchar](10) NOT NULL,
	[Brand] [varchar](50) NULL,
	[Year] [int] NULL,
	[Model] [varchar](50) NULL,
	[Type] [varchar](50) NULL,
	[ReadyStatus] [bit] NULL,
	[Status] [bit] NULL,
	[Parts] [varchar](max) NULL,
	[LastMaintenanceDate] [datetime] NULL,
	[MaintenanceCycle] [int] NULL,
 CONSTRAINT [PK_Vehicle] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AssignmentDetail]  WITH CHECK ADD  CONSTRAINT [FK_AssignmentDetail_OrderInfo] FOREIGN KEY([OrderInfoID])
REFERENCES [dbo].[OrderInfo] ([ID])
GO
ALTER TABLE [dbo].[AssignmentDetail] CHECK CONSTRAINT [FK_AssignmentDetail_OrderInfo]
GO
ALTER TABLE [dbo].[AssignmentDetail]  WITH CHECK ADD  CONSTRAINT [FK_AssignmentDetail_ShippingAssignment] FOREIGN KEY([ShippingAssignmentlID])
REFERENCES [dbo].[ShippingAssignment] ([ID])
GO
ALTER TABLE [dbo].[AssignmentDetail] CHECK CONSTRAINT [FK_AssignmentDetail_ShippingAssignment]
GO
ALTER TABLE [dbo].[MaintenanceDetail]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceDetail_MaintenanceInfo] FOREIGN KEY([MaintenanceInfoID])
REFERENCES [dbo].[MaintenanceInfo] ([ID])
GO
ALTER TABLE [dbo].[MaintenanceDetail] CHECK CONSTRAINT [FK_MaintenanceDetail_MaintenanceInfo]
GO
ALTER TABLE [dbo].[MaintenanceInfo]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceInfo_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[MaintenanceInfo] CHECK CONSTRAINT [FK_MaintenanceInfo_Employee]
GO
ALTER TABLE [dbo].[MaintenanceInfo]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceInfo_Vehicle] FOREIGN KEY([VehicleID])
REFERENCES [dbo].[Vehicle] ([ID])
GO
ALTER TABLE [dbo].[MaintenanceInfo] CHECK CONSTRAINT [FK_MaintenanceInfo_Vehicle]
GO
ALTER TABLE [dbo].[OrderInfo]  WITH CHECK ADD  CONSTRAINT [FK_ShippingInfo_Destination] FOREIGN KEY([DestinationID])
REFERENCES [dbo].[Company] ([ID])
GO
ALTER TABLE [dbo].[OrderInfo] CHECK CONSTRAINT [FK_ShippingInfo_Destination]
GO
ALTER TABLE [dbo].[OrderInfo]  WITH CHECK ADD  CONSTRAINT [FK_ShippingInfo_Source] FOREIGN KEY([SourceID])
REFERENCES [dbo].[Company] ([ID])
GO
ALTER TABLE [dbo].[OrderInfo] CHECK CONSTRAINT [FK_ShippingInfo_Source]
GO
ALTER TABLE [dbo].[ShippingAssignment]  WITH CHECK ADD  CONSTRAINT [FK_ShippingAssignment_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[ShippingAssignment] CHECK CONSTRAINT [FK_ShippingAssignment_Employee]
GO
ALTER TABLE [dbo].[ShippingAssignment]  WITH CHECK ADD  CONSTRAINT [FK_ShippingAssignment_Vehicle] FOREIGN KEY([VehicleID])
REFERENCES [dbo].[Vehicle] ([ID])
GO
ALTER TABLE [dbo].[ShippingAssignment] CHECK CONSTRAINT [FK_ShippingAssignment_Vehicle]
GO
USE [master]
GO
ALTER DATABASE [TCMS_Database] SET  READ_WRITE 
GO
