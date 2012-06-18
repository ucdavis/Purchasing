USE [PrePurchasing]
GO

-- =============================================
-- Author:		Ken Taylor
-- Create date: February 8, 2012
-- Description:	Create the vAccounts Partition Tables
-- =============================================
ALTER PROCEDURE usp_CreateAccountsPartitionTable
	-- Add the parameters for the stored procedure here
	@TableNamePrefix varchar(255) = 'vAccountsPartitionTable', --Default table name prefix
	@ReferentialTableNamePrefix varchar(255) = 'vOrganizationsPartitionTable', --vOrganizations table name prefix that is referenced by vAccounts table
	@Mode smallint = 1, --0 to create all three tables; 1 to create main table only (default); 2 to create load tables only.
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
DECLARE @TableNamePrefix varchar(255) = 'vAccountsPartitionTable'
DECLARE @ReferentialTableNamePrefix varchar(255) = 'vOrganizationsPartitionTable'
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
			IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @TableNamePrefix + @TableNameSuffix + '_' + @ReferentialTableNamePrefix + @TableNameSuffix + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @TableNamePrefix + @TableNameSuffix + ']''))
			ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '] DROP CONSTRAINT [FK_' + @TableNamePrefix + @TableNameSuffix + '_' + @ReferentialTableNamePrefix + @TableNameSuffix + ']

			IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N''[DF_' + @TableNamePrefix + @TableNameSuffix + '_IsAct_22AA2996]'') AND type = ''D'')
			BEGIN
				ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '] DROP CONSTRAINT [DF_' + @TableNamePrefix + @TableNameSuffix + '_IsAct_22AA2996]
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
				[IsActive] [bit] NOT NULL,
				[AccountManager] [varchar](30) NULL,
				[AccountManagerId] [varchar](10) NULL,
				[PrincipalInvestigator] [varchar](30) NULL,
				[PrincipalInvestigatorId] [varchar](10) NULL,
				[OrganizationId] [varchar](10) NOT NULL,
				[PartitionColumn] [int] NOT NULL,
				CONSTRAINT [PK_' + @TableNamePrefix + @TableNameSuffix + '] PRIMARY KEY CLUSTERED 
			(
				[Id] ASC,
				[PartitionColumn] ASC
			)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF)
			) ON EvenOddPartitionScheme (PartitionColumn)
			 
			SET ANSI_PADDING ON
		'
		IF @TableNameSuffix LIKE ''
		BEGIN
			SELECT @TSQL += '
			ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @TableNamePrefix + @TableNameSuffix + '_' + @ReferentialTableNamePrefix + @TableNameSuffix + '] FOREIGN KEY([OrganizationId], [PartitionColumn])
			REFERENCES [dbo].[' + @ReferentialTableNamePrefix + @TableNameSuffix + '] ([Id], [PartitionColumn])

			ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '] CHECK CONSTRAINT [FK_' + @TableNamePrefix + @TableNameSuffix + '_' + @ReferentialTableNamePrefix +@TableNameSuffix + '] 
		'
		END
		--ELSE IF @TableNameSuffix LIKE '_Load'
		--BEGIN
		--	SELECT @TSQL += '
		--	ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @TableNamePrefix + @TableNameSuffix + '_' + @ReferentialTableNamePrefix + '] FOREIGN KEY([OrganizationId], [PartitionColumn])
		--	REFERENCES  [dbo].[' + @ReferentialTableNamePrefix + '] ([Id], [PartitionColumn])

		--	ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '] CHECK CONSTRAINT [FK_' + @TableNamePrefix + @TableNameSuffix + '_' + @ReferentialTableNamePrefix + '] 
		--'
		--END
		
		SELECT @TSQL += '
			ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '] ADD CONSTRAINT [DF_' + @TableNamePrefix + @TableNameSuffix + '_IsAct_22AA2996]  DEFAULT ((0)) FOR [IsActive]
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
GO
