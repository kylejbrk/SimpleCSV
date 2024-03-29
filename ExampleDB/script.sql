USE [BluthCompany]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 6/23/2019 4:58:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[AddressID] [int] NOT NULL,
	[CustomerID] [int] NULL,
	[Address1] [nvarchar](max) NULL,
	[Address2] [nvarchar](max) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[PostalCode] [int] NULL,
	[Date] [date] NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[AddressID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 6/23/2019 4:58:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[FileName] [varchar](max) NULL,
	[CustomerID] [int] NOT NULL,
	[LastName] [varchar](50) NULL,
	[FirstName] [nvarchar](50) NULL,
	[MiddleName] [nvarchar](50) NULL,
	[Suffix] [nvarchar](10) NULL,
	[Phone] [nvarchar](15) NULL,
	[Email] [nvarchar](max) NULL,
	[Date] [date] NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 6/23/2019 4:58:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] NOT NULL,
	[CustomerID] [int] NULL,
	[ProductName] [nvarchar](max) NULL,
	[Date] [date] NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TableNamesList]    Script Date: 6/23/2019 4:58:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableNamesList](
	[Table] [nvarchar](50) NOT NULL,
	[ListName] [nvarchar](50) NULL,
	[ImportType] [nvarchar](50) NULL,
	[FNameAdded] [nvarchar](1) NULL,
 CONSTRAINT [PK_TableNamesList] PRIMARY KEY CLUSTERED 
(
	[Table] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Insert Values Into Table [dbo].[TableNamesList] ******/
INSERT INTO [dbo].TableNamesList ([Table], [ListName], [FNameAdded])
VALUES ('Address', 'Address', 'N')

INSERT INTO [dbo].TableNamesList ([Table], [ListName], [FNameAdded])
VALUES ('Customer', 'Customer', 'Y')

INSERT INTO [dbo].TableNamesList ([Table], [ListName], [FNameAdded])
VALUES ('Orders', 'Orders', 'N')

/****** Object:  Table [dbo].[TriggerFired]    Script Date: 6/23/2019 4:58:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TriggerFired](
	[Fired] [nvarchar](50) NULL,
	[Date] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TriggerFired] ADD  CONSTRAINT [DF_TriggerFired_Date]  DEFAULT (getdate()) FOR [Date]
GO
ALTER TABLE [dbo].[Address]  WITH NOCHECK ADD  CONSTRAINT [FK_Address_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Customer]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Orders] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Orders]
GO
/****** Object:  StoredProcedure [dbo].[ClearTables]    Script Date: 6/23/2019 4:58:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kyle Burke
-- Create date: 7/9/2018
-- Description:	Clears all Production Tables
-- =============================================
CREATE PROCEDURE [dbo].[ClearTables] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
Delete
FROM [BluthCompany].[dbo].[Address]

Delete
FROM [BluthCompany].[dbo].[Orders]

Delete
FROM [BluthCompany].[dbo].[Customer]

Delete
FROM [BluthCompany].[dbo].[TriggerFired]

END
GO
