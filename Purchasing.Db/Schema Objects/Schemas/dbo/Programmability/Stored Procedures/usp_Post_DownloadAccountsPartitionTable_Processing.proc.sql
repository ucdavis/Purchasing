-- =============================================
-- Author:		Ken Taylor
-- Create date: February 16, 2012
-- Description:	Restoring Organizations FK after downloading account data and swapping with load table.
-- =============================================
CREATE PROCEDURE [dbo].[usp_Post_DownloadAccountsPartitionTable_Processing]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vAccountsPartitionTable', --Table name of load table being loaded 
	@ReferentialTableName varchar(244) = 'vOrganizationsPartitionTable', --Name of Organizations table being referenced 
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
	-- This is where the issue is: --------------------------
	-- Before we can switch today''s load table partition with today''s main table partition, we have to handle
	-- the load table''s FK reference/constraint against the Organizations table.
	-- Normally this would be against the Organizations load table; however, this is not possible since the Organizations load
	-- table does not exist; therefore, we create the FK against the main organizations table instead, and this technique seems
	-- to work and allow us to swap partitions:
	
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @LoadTableName + '_' + @ReferentialTableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']''))
		ALTER TABLE [dbo].[' + @LoadTableName + '] DROP CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + ']
	
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND type in (N''U''))	
	BEGIN	
		ALTER TABLE [dbo].[' + @LoadTableName + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + '] FOREIGN KEY([OrganizationId], [PartitionColumn])
			REFERENCES [dbo].[' + @ReferentialTableName + '] ([Id], [PartitionColumn])

		ALTER TABLE [dbo].[' + @LoadTableName + '] CHECK CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + '] 
	END
	
	-- Do the same thing for the main accounts and organizations tables:
	IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @TableName + '_' + @ReferentialTableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @TableName + ']''))
		ALTER TABLE [dbo].[' + @TableName + '] DROP CONSTRAINT [FK_' + @TableName + '_' + @ReferentialTableName + ']
		
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @TableName + ']'') AND type in (N''U''))	
	BEGIN	
		ALTER TABLE [dbo].[' + @TableName + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @TableName + '_' + @ReferentialTableName + '] FOREIGN KEY([OrganizationId], [PartitionColumn])
			REFERENCES [dbo].[' + @ReferentialTableName + '] ([Id], [PartitionColumn])

		ALTER TABLE [dbo].[' + @TableName + '] CHECK CONSTRAINT [FK_' + @TableName + '_' + @ReferentialTableName + '] 
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