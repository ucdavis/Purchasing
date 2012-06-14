-- =============================================
-- Author:		Ken Taylor
-- Create date: February 22, 2012
-- Description:	Deleting Organizations FKs prior to downloading organizations data and swapping with load table.
-- =============================================
CREATE PROCEDURE [dbo].[usp_Pre_DownloadOrganizationsPartitionTable_Processing_bak]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vOrganizations_Load', --Table name of load table being loaded 
	@ReferentialTableName varchar(244) = 'vAccounts', --Name of Accounts table being referenced 
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server.
	@PartitionColumn char(1) = 0, --Number to use for partition column
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TableName varchar(255) = SUBSTRING(@LoadTableName, 0, CHARINDEX('_Load',@LoadTableName ))
	DECLARE @TSQL varchar(MAX) = ''

    -- Insert statements for procedure here
	SELECT @TSQL = '
	-- Remove any referential table constraints from the main table:
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @ReferentialTableName + '_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @ReferentialTableName + ']''))
		ALTER TABLE [dbo].[' + @ReferentialTableName + '] DROP CONSTRAINT [FK_' + @ReferentialTableName + '_' + @TableName + ']
	
	-- Do the same for all the other tables with FK references to the main table:
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_ConditionalApproval_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[ConditionalApproval]''))
		ALTER TABLE [dbo].[ConditionalApproval] DROP CONSTRAINT [FK_ConditionalApproval_' + @TableName + ']

	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_WorkgroupsXOrganizations_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[WorkgroupsXOrganizations]''))
		ALTER TABLE [dbo].[WorkgroupsXOrganizations] DROP CONSTRAINT [FK_WorkgroupsXOrganizations_' + @TableName + ']
		
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_CustomFields_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[CustomFields]''))
		ALTER TABLE [dbo].[CustomFields] DROP CONSTRAINT [FK_CustomFields_' + @TableName + ']

	-- Lastly remove the main table''s FK index:
	IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[' + @TableName + ']'') AND name = N''' + @TableName + '_Id_UDX'')
		DROP INDEX [' + @TableName + '_Id_UDX] ON [dbo].[' + @TableName + '] WITH ( ONLINE = OFF )

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