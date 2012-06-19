
-- =============================================
-- Author:		Ken Taylor
-- Create date: May 31, 2012
-- Description:	Create the CommodityGroups Partition Tables
-- =============================================
create PROCEDURE [dbo].[usp_CreateCommodityGroupsPartitionTable]
	-- Add the parameters for the stored procedure here
	@TableNamePrefix varchar(255) = 'vCommodityGroupsPartitionTable', --Default table name prefix
	@Mode smallint = 1, --0 to create all three tables; 1 to create main table only (default); 2 to create load tables only.
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
DECLARE @TableNamePrefix varchar(255) = 'vCommodityGroupsPartitionTable'
DECLARE @IsDebug bit = 0
*/

	SET NOCOUNT ON --Keeps from displaying (3 row(s) affected) message after inserting table names into @TableNameTable.
	--SET NOCOUNT OFF
	DECLARE @TableNameSuffixTable TABLE (TableNameSuffix varchar(10))
    IF @Mode = 1
		INSERT INTO @TableNameSuffixTable VALUES ('')
	ELSE IF @Mode = 2
		INSERT INTO @TableNameSuffixTable VALUES ('_Load'), ('_Empty')
	ELSE
		INSERT INTO @TableNameSuffixTable VALUES (''), ('_Load'), ('_Empty')

	DECLARE @TSQL varchar(MAX) = ''

	DECLARE @TableNameSuffix varchar(10) = ''
	DECLARE myCursor CURSOR FOR SELECT TableNameSuffix FROM @TableNameSuffixTable

	OPEN myCursor

	FETCH NEXT FROM myCursor INTO @TableNameSuffix

	WHILE @@FETCH_STATUS <> -1
	BEGIN 
		SELECT @TSQL = '
			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @TableNamePrefix + @TableNameSuffix + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '](
				[Id] [uniqueidentifier] NOT NULL,
				[GroupCode] [varchar](4) NOT NULL,
				[Name] [varchar](40) NOT NULL,
				[SubGroupCode] [varchar](2) NOT NULL,
				[SubGroupName] [varchar](40) NOT NULL,
				[PartitionColumn] INT NOT NULL,
				CONSTRAINT [PK_' + @TableNamePrefix + @TableNameSuffix + '] PRIMARY KEY CLUSTERED
			(
				[Id] ASC,
				[PartitionColumn] ASC
			) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF))
			ON EvenOddPartitionScheme (PartitionColumn)

			SET ANSI_PADDING OFF

			EXEC sys.sp_addextendedproperty @name=N''MS_Description'', @value=N''Commodity group code'' , @level0type=N''SCHEMA'',@level0name=N''dbo'', @level1type=N''TABLE'',@level1name=N'''+ @TableNamePrefix + @TableNameSuffix +''', @level2type=N''COLUMN'',@level2name=N''Id''
			EXEC sys.sp_addextendedproperty @name=N''MS_Description'', @value=N''Commodity group code'' , @level0type=N''SCHEMA'',@level0name=N''dbo'', @level1type=N''TABLE'',@level1name=N'''+ @TableNamePrefix + @TableNameSuffix +''', @level2type=N''COLUMN'',@level2name=N''GroupCode''
			ALTER TABLE [dbo].['+ @TableNamePrefix + @TableNameSuffix +'] ADD  CONSTRAINT [DF_'+ @TableNamePrefix + @TableNameSuffix +'_Id]  DEFAULT (newid()) FOR [Id]
	'

		IF @IsDebug = 1 
			PRINT @TSQL 
		ELSE 
			EXEC(@TSQL)
			
		FETCH NEXT FROM myCursor INTO @TableNameSuffix
	END

	CLOSE myCursor
	DEALLOCATE myCursor
END