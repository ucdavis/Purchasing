-- =============================================
-- Author:		Ken Taylor
-- Create date: February 8, 2012
-- Description:	Create the vAccounts Partition Tables
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateAccountsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vAccounts', --Default table name
	@ReferentialTableName varchar(255) = 'vOrganizations', --vOrganizations table name that is referenced by vAccounts table
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
DECLARE @LoadTableName varchar(255) = 'vAccounts'
DECLARE @ReferentialTableName varchar(255) = 'vOrganizations'
DECLARE @IsDebug bit = 0
*/

	SET NOCOUNT ON --Keeps from displaying (3 row(s) affected) message after inserting table names into @TableNameTable.
	--SET NOCOUNT OFF

	DECLARE @TSQL varchar(MAX) = ''

	BEGIN 
		SELECT @TSQL = '
			IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @LoadTableName + '_' + @ReferentialTableName  + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']''))
			ALTER TABLE [dbo].[' + @LoadTableName + '] DROP CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + ']

			IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N''[DF_' + @LoadTableName + '_IsAct_22AA2996]'') AND type = ''D'')
			BEGIN
				ALTER TABLE [dbo].[' + @LoadTableName + '] DROP CONSTRAINT [DF_' + @LoadTableName + '_IsAct_22AA2996]
			END

			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @LoadTableName + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @LoadTableName + '](
				[Id] [varchar](10) NOT NULL,
				[Name] [varchar](50) NOT NULL,
				[IsActive] [bit] NOT NULL,
				[AccountManager] [varchar](30) NULL,
				[AccountManagerId] [varchar](10) NULL,
				[PrincipalInvestigator] [varchar](30) NULL,
				[PrincipalInvestigatorId] [varchar](10) NULL,
				[OrganizationId] [varchar](10) NOT NULL,
				CONSTRAINT [PK_' + @LoadTableName + '] PRIMARY KEY CLUSTERED 
			(
				[Id] ASC
			)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
			) ON [PRIMARY]
			 
			SET ANSI_PADDING ON
		'
		SELECT @TSQL += '
			ALTER TABLE [dbo].[' + @LoadTableName + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + '] FOREIGN KEY([OrganizationId])
			REFERENCES [dbo].[' + @ReferentialTableName + '] ([Id])

			ALTER TABLE [dbo].[' + @LoadTableName + '] CHECK CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + '] 
		'
		END
		
		SELECT @TSQL += '
			ALTER TABLE [dbo].[' + @LoadTableName + '] ADD CONSTRAINT [DF_' + @LoadTableName + '_IsAct_22AA2996]  DEFAULT ((0)) FOR [IsActive]
		'

		IF @IsDebug = 1 
			PRINT @TSQL 
		ELSE 
			EXEC(@TSQL)
			
END