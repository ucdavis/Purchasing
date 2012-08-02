-- =============================================
-- Author:		Ken Taylor
-- Create date: May 31, 2012
-- Description:	Create the CommodityGroups table.
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateCommodityGroupsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vCommodityGroups', --Default table name.
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
DECLARE @LoadTableName varchar(255) = 'vCommodityGroups'
DECLARE @IsDebug bit = 0
*/

	SET NOCOUNT ON --Keeps from displaying (3 row(s) affected) message after inserting table names into @TableNameTable.
	--SET NOCOUNT OFF

	DECLARE @TSQL varchar(MAX) = ''

		SELECT @TSQL = '
			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @LoadTableName + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @LoadTableName + '](
				[Id] [uniqueidentifier] NOT NULL,
				[GroupCode] [varchar](4) NOT NULL,
				[Name] [varchar](40) NOT NULL,
				[SubGroupCode] [varchar](2) NOT NULL,
				[SubGroupName] [varchar](40) NOT NULL,
				
				CONSTRAINT [PK_' + @LoadTableName + '] PRIMARY KEY CLUSTERED
			(
				[Id] ASC
			) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			SET ANSI_PADDING OFF

			EXEC sys.sp_addextendedproperty @name=N''MS_Description'', @value=N''Commodity group code'' , @level0type=N''SCHEMA'',@level0name=N''dbo'', @level1type=N''TABLE'',@level1name=N'''+ @LoadTableName +''', @level2type=N''COLUMN'',@level2name=N''Id''
			EXEC sys.sp_addextendedproperty @name=N''MS_Description'', @value=N''Commodity group code'' , @level0type=N''SCHEMA'',@level0name=N''dbo'', @level1type=N''TABLE'',@level1name=N'''+ @LoadTableName +''', @level2type=N''COLUMN'',@level2name=N''GroupCode''
			ALTER TABLE [dbo].['+ @LoadTableName +'] ADD  CONSTRAINT [DF_'+ @LoadTableName +'_Id]  DEFAULT (newid()) FOR [Id]
	'

		IF @IsDebug = 1 
			PRINT @TSQL 
		ELSE 
			EXEC(@TSQL)
END