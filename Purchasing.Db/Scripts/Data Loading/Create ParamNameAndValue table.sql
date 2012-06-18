USE [PrePurchasing]
GO

/****** Object:  Table [dbo].[ParamNameAndValue]    Script Date: 02/14/2012 15:34:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ParamNameAndValue](
	[ParamName] [varchar](255) NOT NULL,
	[ParamValue] [varchar](255) NOT NULL,
	[Description] [varchar](1024) NULL,
 CONSTRAINT [PK_TableName_1] PRIMARY KEY CLUSTERED 
(
	[ParamName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Table Name Variable Name: The name of the table variable name corresponding to the table name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ParamNameAndValue', @level2type=N'COLUMN',@level2name=N'ParamName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Table Name: The name, prefix, midfix or suffix of the table' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ParamNameAndValue', @level2type=N'COLUMN',@level2name=N'ParamValue'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Description: A description of the table''s intended use' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ParamNameAndValue', @level2type=N'COLUMN',@level2name=N'Description'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Table Name: A table containing all of the table names or table name prefixes, midfixes or suffixes for the AD419 report tables ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ParamNameAndValue'
GO


