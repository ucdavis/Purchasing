USE [PrePurchasing]
GO
/****** Object:  StoredProcedure [dbo].[usp_DownloadOrganizationsPartitionTable]    Script Date: 02/13/2012 09:12:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download organizational data and ultimately load into the vOrganizationsPartitionTable
-- =============================================
ALTER PROCEDURE [dbo].[usp_DownloadOrganizationsPartitionTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vOrganizationsPartitionTable_Load', --Load table name, i.e. prefix, of load table being loaded 
	@ReferentialTableName varchar(244) = 'vAccountsPartitionTable', --Name of accounts table being referenced 
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server.
	@PartitionColumn char(1) = 0, --Number to use for partition column
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TableName varchar(255) = SUBSTRING(@LoadTableName, 0, CHARINDEX('_Load',@LoadTableName ))
	DECLARE @ReferentialLoadTableName varchar(255) = @ReferentialTableName + '_Load'
	
	DECLARE @TSQL varchar(MAX) = ''

    -- Insert statements for procedure here
	SELECT @TSQL = '
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @ReferentialLoadTableName + '_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @ReferentialLoadTableName + ']''))
		ALTER TABLE [PrePurchasing].[dbo].[' + @ReferentialLoadTableName + '] DROP CONSTRAINT [FK_' + @ReferentialLoadTableName + '_' + @TableName + ']

	TRUNCATE TABLE [PrePurchasing].[dbo].[' + @LoadTableName + ']
	
--	ALTER TABLE [dbo].[' + @ReferentialLoadTableName + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @ReferentialLoadTableName + '_' + @TableName + '] FOREIGN KEY([OrganizationId], [PartitionColumn])
--REFERENCES [dbo].[' + @TableName + '] ([Id], [PartitionColumn])
--	ALTER TABLE [dbo].[' + @ReferentialLoadTableName + '] CHECK CONSTRAINT [FK_' + @ReferentialLoadTableName + '_' + @TableName + '] 
	
	INSERT INTO ' + @LoadTableName + ' SELECT DISTINCT
		[Id]
		,ISNULL ([Name], [Id]) AS [Name]
		,[TypeCode]
		,[TypeName]
		,[ParentId]
		,(CASE [IsActive] WHEN ''Y'' THEN 1 ELSE 0 END) AS [IsActive]
      , ' + Convert(char(1), @PartitionColumn) + ' AS [PartitionColumn]
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT DISTINCT
			O.CHART_NUM || ''''-'''' || O.ORG_ID AS Id,
			O.ORG_NAME AS Name,
			O.ORG_TYPE_CODE AS TypeCode,
			O.org_type_name AS TypeName,
			O.CHART_NUM || ''''-'''' || O.rpts_to_org_id AS ParentId,
			O.ACTIVE_IND AS IsActive
		FROM FINANCE.ORGANIZATION_HIERARCHY O
		WHERE O.FISCAL_YEAR = 9999 AND O.FISCAL_PERIOD = ''''--'''' 
	'')
	
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @ReferentialLoadTableName + '_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @ReferentialLoadTableName + ']''))
		ALTER TABLE [dbo].[' + @ReferentialLoadTableName + '] DROP CONSTRAINT [FK_' + @ReferentialLoadTableName + '_' + @TableName + ']

	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @ReferentialTableName + '_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @ReferentialTableName + ']''))
		ALTER TABLE [dbo].[' + @ReferentialTableName + '] DROP CONSTRAINT [FK_' + @ReferentialTableName + '_' + @TableName + ']
	'
	
	-------------------------------------------------------------------------
	if @IsDebug = 1
		BEGIN
			--used for testing
			PRINT @TSQL	
		END
	else
		BEGIN
			--Execute the command:
			EXEC(@TSQL)
		END 
END
