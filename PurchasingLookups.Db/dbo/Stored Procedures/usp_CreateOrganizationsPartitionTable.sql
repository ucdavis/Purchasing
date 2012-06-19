
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 8, 2012
-- Description:	Create the vOrganizations Partition Tables
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateOrganizationsPartitionTable]
	-- Add the parameters for the stored procedure here
	@TableNamePrefix varchar(255) = 'vOrganizationsPartitionTable', --Default vOrganizations table name prefix
	@ReferentialTableNamePrefix varchar(255) = 'vAccountsPartitionTable', --vAccounts table name prefix that references vOrganizations table
	@Mode smallint = 1, --0 to create all three tables; 1 to create main table only (default); 2 to create load tables only.
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
	DECLARE @TableNamePrefix varchar(255) = 'vOrganizationsPartitionTable'
	DECLARE @ReferentialTableNamePrefix varchar(255) = 'vAccountsPartitionTable'
	DECLARE @IsDebug bit = 0
*/
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;  --Keeps from displaying (3 row(s) affected) message after inserting table names into @TableNameTable.
	--SET NOCOUNT OFF

	DECLARE @TableNameSuffixTable TABLE (TableName varchar(255))
	IF @Mode = 1
		INSERT INTO @TableNameSuffixTable VALUES ('')
	ELSE IF @Mode = 2
		INSERT INTO @TableNameSuffixTable VALUES ('_Load'), ('_Empty')
	ELSE
		INSERT INTO @TableNameSuffixTable VALUES (''), ('_Load'), ('_Empty')
	DECLARE @TSQL varchar(MAX) = ''
	DECLARE @TableNameSuffix varchar(10) = '' 

	DECLARE myCursor CURSOR FOR SELECT TableName FROM @TableNameSuffixTable

	OPEN myCursor

	FETCH NEXT FROM myCursor INTO @TableNameSuffix

	WHILE @@FETCH_STATUS <> -1
	BEGIN 

		SELECT @TSQL = '
			IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @ReferentialTableNamePrefix + @TableNameSuffix +'_' + @TableNamePrefix + @TableNameSuffix  + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @ReferentialTableNamePrefix + @TableNameSuffix + ']''))
			BEGIN
				ALTER TABLE [PrePurchasingLookups].[dbo].['+ @ReferentialTableNamePrefix + @TableNameSuffix+ '] DROP CONSTRAINT [FK_' + @ReferentialTableNamePrefix + @TableNameSuffix +'_' + @TableNamePrefix + @TableNameSuffix + ']
			END

			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @TableNamePrefix + @TableNameSuffix + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '](
				[Id] [varchar](10) NOT NULL,
				[Name] [varchar](50) NOT NULL,
				[TypeCode] [char](1) NOT NULL,
				[TypeName] [varchar](50) NOT NULL,
				[ParentId] [varchar](10) NULL,
				[IsActive] [bit] NOT NULL,
				[PartitionColumn] [int] NOT NULL,
				CONSTRAINT [PK_' + @TableNamePrefix + @TableNameSuffix + '] PRIMARY KEY CLUSTERED
			(
				[Id] ASC, [PartitionColumn] ASC
			) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF))
			ON EvenOddPartitionScheme (PartitionColumn)

			SET ANSI_PADDING ON
		'

		IF @IsDebug = 1 PRINT @TSQL ELSE EXEC(@TSQL)
		FETCH NEXT FROM myCursor INTO @TableNameSuffix
	END

	CLOSE myCursor
	DEALLOCATE myCursor	
END