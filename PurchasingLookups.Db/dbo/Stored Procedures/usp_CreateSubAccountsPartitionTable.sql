
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 8, 2012
-- Description:	Create the SubAccounts Partition Tables
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateSubAccountsPartitionTable]
	-- Add the parameters for the stored procedure here
	@TableNamePrefix varchar(255) = 'vSubAccountsPartitionTable', --Default table name prefix
	@Mode smallint = 1, --0 to create all three tables; 1 to create main table only (default); 2 to create load tables only.
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
DECLARE @TableNamePrefix varchar(255) = 'vSubAccountsPartitionTable'
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
			IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N''[DF_' + @TableNamePrefix + @TableNameSuffix + '_id]'') AND type = ''D'')
			BEGIN
				ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '] DROP CONSTRAINT [DF_' + @TableNamePrefix + @TableNameSuffix + '_id]
			END

			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @TableNamePrefix + @TableNameSuffix + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '](
				[id] [uniqueidentifier] NOT NULL,
				[AccountNumber] [varchar](10) NOT NULL,
				[SubAccountNumber] [varchar](5) NOT NULL,
				[Name] [varchar](40) NULL,
				[IsActive] [bit] NOT NULL,
				[PartitionColumn] INT NOT NULL,
				CONSTRAINT [PK_' + @TableNamePrefix + @TableNameSuffix + '] PRIMARY KEY CLUSTERED
			(
				[AccountNumber] ASC,
				[SubAccountNumber] ASC,
				[PartitionColumn] ASC
			) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF))
			ON EvenOddPartitionScheme (PartitionColumn)

			SET ANSI_PADDING ON

			ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '] ADD  CONSTRAINT [DF_' + @TableNamePrefix + @TableNameSuffix + '_id]  DEFAULT (newid()) FOR [id]	
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