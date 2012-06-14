-- =============================================
-- Author:		Ken Taylor
-- Create date: February 22, 2012
-- Description:	Restoring Organizations FKs after downloading organizations data and swapping with load table.
-- =============================================
CREATE PROCEDURE [dbo].[usp_Post_DownloadOrganizationsPartitionTable_Processing_bak]
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
	-- First recreate main table FK index:
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[' + @TableName + ']'') AND name = N''' + @TableName + '_Id_UDX'')
	BEGIN
		CREATE UNIQUE NONCLUSTERED INDEX [' + @TableName + '_Id_UDX] ON [dbo].[' + @TableName + '] 
		(
			[Id] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	END
	
	-- Then recreate the FKs and check constraints:
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_CustomFields_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[CustomFields]''))
	BEGIN
		ALTER TABLE [dbo].[CustomFields]  WITH CHECK ADD  CONSTRAINT [FK_CustomFields_' + @TableName + '] FOREIGN KEY([OrganizationId])
			REFERENCES [dbo].[' + @TableName + '] ([Id])
			
		ALTER TABLE [dbo].[CustomFields] CHECK CONSTRAINT [FK_CustomFields_' + @TableName + ']	
	END
		
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_WorkgroupsXOrganizations_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[WorkgroupsXOrganizations]''))
	BEGIN
		ALTER TABLE [dbo].[WorkgroupsXOrganizations]  WITH CHECK ADD  CONSTRAINT [FK_WorkgroupsXOrganizations_' + @TableName + '] FOREIGN KEY([OrganizationId])
			REFERENCES [dbo].[' + @TableName + '] ([Id])
			
		ALTER TABLE [dbo].[WorkgroupsXOrganizations] CHECK CONSTRAINT [FK_WorkgroupsXOrganizations_' + @TableName + ']
	END
	
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_ConditionalApproval_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[ConditionalApproval]''))
	BEGIN
		ALTER TABLE [dbo].[ConditionalApproval]  WITH CHECK ADD  CONSTRAINT [FK_ConditionalApproval_' + @TableName + '] FOREIGN KEY([OrganizationId])
			REFERENCES [dbo].[' + @TableName + '] ([Id])
			
		ALTER TABLE [dbo].[ConditionalApproval] CHECK CONSTRAINT [FK_ConditionalApproval_' + @TableName + ']
	END
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