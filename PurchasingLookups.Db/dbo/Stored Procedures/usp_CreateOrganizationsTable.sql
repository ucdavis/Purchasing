-- =============================================
-- Author:		Ken Taylor
-- Create date: February 8, 2012
-- Description:	Create the vOrganizations table.
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateOrganizationsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vOrganizations', --Default vOrganizations table name.
	@ReferentialTableName varchar(255) = 'vAccounts', --vAccounts table name that references vOrganizations table
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
	DECLARE @LoadTableName varchar(255) = 'vOrganizations'
	DECLARE @ReferentialTableName varchar(255) = 'vAccounts'
	DECLARE @IsDebug bit = 0
*/
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;  --Keeps from displaying (3 row(s) affected) message after inserting table names into @TableNameTable.
	--SET NOCOUNT OFF

	DECLARE @TSQL varchar(MAX) = '' 

		SELECT @TSQL = '
			IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @ReferentialTableName  + '_' + @LoadTableName   + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @ReferentialTableName  + ']''))
			BEGIN
				ALTER TABLE [PrePurchasingLookups].[dbo].['+ @ReferentialTableName + '] DROP CONSTRAINT [FK_' + @ReferentialTableName  + '_' + @LoadTableName  + ']
			END

			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName  + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @LoadTableName  + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @LoadTableName  + '](
				[Id] [varchar](10) NOT NULL,
				[Name] [varchar](50) NOT NULL,
				[TypeCode] [char](1) NOT NULL,
				[TypeName] [varchar](50) NOT NULL,
				[ParentId] [varchar](10) NULL,
				[IsActive] [bit] NOT NULL,
				CONSTRAINT [PK_' + @LoadTableName  + '] PRIMARY KEY CLUSTERED
			(
				[Id] ASC
			) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
			) ON [PRIMARY]

			SET ANSI_PADDING ON
		'

		IF @IsDebug = 1 
			PRINT @TSQL 
		ELSE 
			EXEC(@TSQL)
END