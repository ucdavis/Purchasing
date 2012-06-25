-- =============================================
-- Author:		Ken Taylor
-- Create date: February 16, 2012
-- Description:	Restoring Organizations FK after downloading account data.
-- Modifications: 2012-03-01 by kjt: Revised to include statements necessary to recreate full-text search catalog and indexes
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_Post_DownloadAccountsTable_Processing]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vAccounts', --Name of table being loaded 
	@ReferentialTableName varchar(244) = 'vOrganizations', --Name of Organizations table being referenced 
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server.
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TSQL varchar(MAX) = ''

    -- Insert statements for procedure here
	SELECT @TSQL = '
	-- This is where the issue is: --------------------------
	-- Before we can switch today''s load table partition with today''s main table partition, we have to handle
	-- the load table''s FK reference/constraint against the Organizations table.
	-- Normally this would be against the Organizations load table; however, this is not possible since the Organizations load
	-- table does not exist; therefore, we create the FK against the main organizations table instead, and this technique seems
	-- to work and allow us to swap partitions:
	
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND name = N''' + @LoadTableName + '_Id_UDX'')
	BEGIN
		CREATE UNIQUE NONCLUSTERED INDEX [' + @LoadTableName + '_Id_UDX] ON [dbo].[' + @LoadTableName + '] 
		(
			[Id] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	END
	
	-- Do the same thing for the main accounts and organizations tables:
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[FK_' + @LoadTableName + '_' + @ReferentialTableName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']''))
	BEGIN	
		ALTER TABLE [dbo].[' + @LoadTableName + ']  WITH CHECK ADD  CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + '] FOREIGN KEY([OrganizationId])
			REFERENCES [dbo].[' + @ReferentialTableName + '] ([Id])

		ALTER TABLE [dbo].[' + @LoadTableName + '] CHECK CONSTRAINT [FK_' + @LoadTableName + '_' + @ReferentialTableName + '] 
	END
	
	-- Recreate the full-text catalog if missing:
	IF NOT EXISTS (SELECT * FROM [PrePurchasingLookups].[sys].[fulltext_catalogs] WHERE [name] LIKE ''' + @LoadTableName + '%'')
		CREATE FULLTEXT CATALOG ' + @LoadTableName +'_IdName_SDX
	
	-- Lastly recreate the table''s full-text search index:
	IF NOT EXISTS (
		SELECT * FROM sys.objects O 
		INNER JOIN sys.fulltext_indexes FTI ON O.object_id = FTI.object_id
		WHERE O.object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND O.type in (N''U'')
	)
	BEGIN
		CREATE FULLTEXT INDEX ON [dbo].[' + @LoadTableName + ']
		([Id] LANGUAGE English, [Name] LANGUAGE English)
		KEY INDEX [' + @LoadTableName + '_Id_UDX] ON (' + @LoadTableName + '_IdName_SDX, FILEGROUP [PRIMARY])
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