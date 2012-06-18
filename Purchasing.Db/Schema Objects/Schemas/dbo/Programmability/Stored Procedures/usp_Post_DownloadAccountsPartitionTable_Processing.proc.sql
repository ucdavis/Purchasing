-- =============================================
-- Author:		Ken Taylor
-- Create date: February 16, 2012
-- Description:	Restoring Organizations FK after downloading account data and swapping with load table.
-- Modifications: 2012-03-01 by kjt: Revised to include statements necessary to recreate full-text search catalog and indexes
-- =============================================
CREATE PROCEDURE [dbo].[usp_Post_DownloadAccountsPartitionTable_Processing]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vAccounts_Load', --Table name of load table being loaded 
	@ReferentialTableName varchar(244) = 'vOrganizations', --Name of Organizations table being referenced 
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
	
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[' + @TableName + ']'') AND name = N''' + @TableName + '_Id_UDX'')
	BEGIN
		CREATE UNIQUE NONCLUSTERED INDEX [' + @TableName + '_Id_UDX] ON [dbo].[' + @TableName + '] 
		(
			[Id] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	END

	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_Splits_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[Splits]''))
	BEGIN
		ALTER TABLE [dbo].[Splits]  WITH CHECK ADD  CONSTRAINT [FK_Splits_' + @TableName + '] FOREIGN KEY([Account])
			REFERENCES [dbo].[' + @TableName + '] ([Id])
			
		ALTER TABLE [dbo].[Splits] CHECK CONSTRAINT [FK_Splits_' + @TableName + ']
	END
	
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_WorkgroupAccounts_' + @TableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[WorkgroupAccounts]''))
	BEGIN
		ALTER TABLE [dbo].[WorkgroupAccounts]  WITH CHECK ADD  CONSTRAINT [FK_WorkgroupAccounts_' + @TableName + '] FOREIGN KEY([AccountId])
			REFERENCES [dbo].[' + @TableName + '] ([Id])
		
		ALTER TABLE [dbo].[WorkgroupAccounts] CHECK CONSTRAINT [FK_WorkgroupAccounts_' + @TableName + ']
	END
	
	-- Do the same thing for the main accounts and organizations tables:
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @TableName + '_' + @ReferentialTableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @TableName + ']''))
	BEGIN	
		ALTER TABLE [dbo].[' + @TableName + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @TableName + '_' + @ReferentialTableName + '] FOREIGN KEY([OrganizationId], [PartitionColumn])
			REFERENCES [dbo].[' + @ReferentialTableName + '] ([Id], [PartitionColumn])

		ALTER TABLE [dbo].[' + @TableName + '] CHECK CONSTRAINT [FK_' + @TableName + '_' + @ReferentialTableName + '] 
	END
	
	-- Recreate the full-text catalog if missing:
	IF NOT EXISTS (SELECT * FROM [PrePurchasing].[sys].[fulltext_catalogs] WHERE [name] LIKE ''' + @TableName + '%'')
		CREATE FULLTEXT CATALOG ' + @TableName +'_IdName_SDX
	
	-- Lastly recreate the table''s full-text search index:
	IF NOT EXISTS (
		SELECT * FROM sys.objects O 
		INNER JOIN sys.fulltext_indexes FTI ON O.object_id = FTI.object_id
		WHERE O.object_id = OBJECT_ID(N''[dbo].[' + @TableName + ']'') AND O.type in (N''U'')
	)
	BEGIN
		CREATE FULLTEXT INDEX ON [dbo].[' + @TableName + ']
		([Id] LANGUAGE English, [Name] LANGUAGE English)
		KEY INDEX [' + @TableName + '_Id_UDX] ON (' + @TableName + '_IdName_SDX, FILEGROUP [PRIMARY])
		WITH (STOPLIST = SYSTEM, CHANGE_TRACKING = AUTO);
	END
	
	-- 
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