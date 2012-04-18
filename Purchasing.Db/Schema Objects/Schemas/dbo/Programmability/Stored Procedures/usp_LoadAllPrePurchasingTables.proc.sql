-- =============================================
-- Author:		Ken Taylor
-- Create date: February 14, 2012
-- Description:	Load all of the new PrePurchasing tables as per Alan Lai
-- Note: 
--		Comment out specific rows in @ParamsTable table to alter which tables are loaded.
--		Change TableName and ReferentialTableName fields in @ParamsTable table to alter names of tables being loaded.
--
-- Usage:
--	USE [PrePurchasing]
--	GO
--
--	DECLARE	@return_value int
-- 
--	EXEC	@return_value = [dbo].[usp_LoadAllPrePurchasingTables] [@LinkedServerName = <some_other_linked_server>] [@IsDebug = 1]
-- 
--	SELECT	'Return Value' = @return_value
-- 
--	GO
--
-- Modifications:
--	2012-02-16 by kjt: Revised to call accounts post load processing sproc.
--  2012-02-22 by kjt: Revised to allow calling of pre-processing sproc.
--	2012-02-23 by kjt: Revised main loop ELSE portion to pass @TableName Vs. @LoadTableName.
--	2012-02-28 by kjt: Revised @LinkedServerName from varchar(10) to varchar(20) to allow for longer server names.
--	2012-02-29 by kjt: Revised to include pre and post scripts for vCommodities.
--	2012-03-01 by kjt: Revised to include pre and post processing script for vVendors
--	2012-03-09 by kjt: Revised to call load sproc for vBuildings as per Alan Lai.
-- =============================================
CREATE PROCEDURE usp_LoadAllPrePurchasingTables 
	-- Add the parameters for the stored procedure here
	@LinkedServerName varchar(20) = '', --Can be changed to whatever the 'full' access linked server connection is. 
	@IsDebug bit = 0 --Set to 1 to display SQL statements
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/*
	DECLARE @LinkedServerName varchar(10) = 'FIS_DS'
    DECLARE @IsDebug bit = 0
*/
	DECLARE	@ReturnVal int
	DECLARE @TableCount int = 0
	DECLARE @SQL_String nvarchar(MAX) = N''
	
	SELECT @LinkedServerName = (SELECT dbo.udf_GetParameterValue(@LinkedServerName, 'FIS_DS', 'LinkedServerName'))

	DECLARE @ParamsTable TABLE (LoadSprocName varchar(255), PreProcessingSprocName varchar(255), PostProcessingSprocName varchar (255), CreateTableSprocName varchar(255), TableName varchar(255), ReferentialTableName varchar(255))
	
	INSERT INTO @ParamsTable VALUES 
		 ('usp_DownloadOrganizationsPartitionTable', 'usp_Pre_DownloadOrganizationsPartitionTable_Processing', 'usp_Post_DownloadOrganizationsPartitionTable_Processing', 'usp_CreateOrganizationsPartitionTable', (SELECT dbo.udf_GetParameterValue(NULL, 'vOrganizations', 'OrganizationsTableName')), (SELECT dbo.udf_GetParameterValue(NULL, 'vAccounts', 'AccountsTableName')))
		,('usp_DownloadAccountsPartitionTable', 'usp_Pre_DownloadAccountsPartitionTable_Processing', 'usp_Post_DownloadAccountsPartitionTable_Processing', 'usp_CreateAccountsPartitionTable', (SELECT dbo.udf_GetParameterValue(NULL, 'vAccounts', 'AccountsTableName')), (SELECT dbo.udf_GetParameterValue(NULL, 'vOrganizations', 'OrganizationsTableName')))
		,('usp_DownloadSubAccountsPartitionTable', '', '', 'usp_CreateSubAccountsPartitionTable', (SELECT dbo.udf_GetParameterValue(NULL, 'vSubAccounts', 'SubAccountsTableName')), '')
		,('usp_DownloadCommoditiesPartitionTable', 'usp_Pre_DownloadCommoditiesPartitionTable_Processing', 'usp_Post_DownloadCommoditiesPartitionTable_Processing', 'usp_CreateCommoditiesPartitionTable', (SELECT dbo.udf_GetParameterValue(NULL, 'vCommodities', 'CommoditiesTableName')), '')
		,('usp_DownloadVendorsPartitionTable', 'usp_Pre_DownloadVendorsPartitionTable_Processing', 'usp_Post_DownloadVendorsPartitionTable_Processing', 'usp_CreateVendorsPartitionTable', (SELECT dbo.udf_GetParameterValue(NULL, 'vVendors', 'VendorsTableName')), '')
		,('usp_DownloadVendorAddressesPartitionTable', '', '', 'usp_CreateVendorAddressesPartitionTable', (SELECT dbo.udf_GetParameterValue(NULL, 'vVendorAddresses', 'VendorAddressesTableName')), '')
		,('usp_DownloadUnitOfMeasuresTable', '', '', '', (SELECT dbo.udf_GetParameterValue(NULL, 'UnitOfMeasures', 'UnitOfMeasuresTableName')), '')
		,('usp_DownloadBuildingsTable', '', '', '', (SELECT dbo.udf_GetParameterValue(NULL, 'vBuildings', 'BuildingsTableName')), '')
		
	IF @IsDebug = 1 
	BEGIN
		SELECT 'Linked Server Name: ' + @LinkedServerName
		SELECT * FROM @ParamsTable
	END
	
	DECLARE @LoadSprocName varchar(255), @PreProcessingSprocName varchar (255), @PostProcessingSprocName varchar (255), @CreateTableSprocName varchar(255), @TableName varchar(255), @ReferentialTableName varchar(255)

	DECLARE @Parameter_Definition nvarchar(MAX) = N'
		@LoadSprocName varchar(255),
		@CreateTableSprocName varchar(255), 
		@TableName varchar(255), 
		@ReferentialTableName varchar(255), 
		@LinkedServerName varchar(20), 
		@IsDebug bit, 
		@return_value int OUTPUT'
		
	DECLARE LoadCursor CURSOR FOR SELECT * FROM @ParamsTable
	OPEN LoadCursor 
	FETCH NEXT FROM LoadCursor INTO @LoadSprocName, @PreProcessingSprocName, @PostProcessingSprocName, @CreateTableSprocName, @TableName, @ReferentialTableName 
	WHILE @@FETCH_STATUS <> -1
	BEGIN
		DECLARE @LoadTableName varchar(255) = @TableName + '_Load' 
		DECLARE @PartitionColumn int = (SELECT dbo.udf_GetEvenOddPartitionNumber(GETDATE()))
	
		IF @PreProcessingSprocName IS NOT NULL AND @PreProcessingSprocName NOT LIKE ''
		BEGIN
			SELECT @SQL_String = N'
				EXEC @return_value = 
				[dbo].[' + @PreProcessingSprocName + ']
					@LoadTableName,
					@ReferentialTableName,
					@LinkedServerName,
					@PartitionColumn,
					@IsDebug;
		'
			EXECUTE sp_executesql @SQL_String, N'
				@LoadTableName varchar(255), 
				@ReferentialTableName varchar(255), 
				@LinkedServerName varchar(20), 
				@PartitionColumn int, 
				@IsDebug bit, 
				@return_value int OUTPUT', 
				@LoadTableName = @LoadTableName, 
				@ReferentialTableName = @ReferentialTableName,
				@LinkedServerName = @LinkedServerName, 
				@PartitionColumn = @PartitionColumn,
				@IsDebug = @IsDebug, 
				@return_value = @ReturnVal OUTPUT
			
			-- SELECT	'Return Value' = @ReturnVal
		END
	
		IF @CreateTableSprocName IS NOT NULL AND @CreateTableSprocName NOT LIKE ''
		  BEGIN
		  -- Handle swap-table loads:
			SELECT @SQL_String = N'
			EXEC @return_value = 
			[dbo].[usp_LoadTableUsingSwapPartitions]
				@LoadSprocName,
				@CreateTableSprocName,
				@TableName,
				@ReferentialTableName,
				@LinkedServerName,
				@IsDebug;
		'
			EXECUTE sp_executesql @SQL_String, @Parameter_Definition, 
			@LoadSprocName = @LoadSprocName, 
			@CreateTableSprocName = @CreateTableSprocName, 
			@TableName = @TableName, 
			@ReferentialTableName = @ReferentialTableName,
			@LinkedServerName = @LinkedServerName, 
			@IsDebug = @IsDebug, 
			@return_value = @ReturnVal OUTPUT
			
			-- SELECT	'Return Value' = @ReturnVal
		  END
		ELSE
		  BEGIN
		  -- Handle non-swap table loads:
		  	DECLARE @StartTime datetime = (SELECT GETDATE())
			DECLARE @TempTime datetime = (SELECT @StartTime)
			DECLARE @EndTime datetime = (SELECT @StartTime)
			IF @IsDebug = 0 PRINT '--Start time for loading ' + @LoadTableName + ': ' +  + CONVERT(varchar(20),@StartTime, 114)

			SELECT @SQL_String = N'
				EXEC @return_value = 
				[dbo].[' + @LoadSprocName + ']
					@LoadTableName,
					@LinkedServerName,
					@PartitionColumn,
					@IsDebug;
		'
		
			EXECUTE sp_executesql @SQL_String, N'
				@LoadTableName varchar(255), 
				@LinkedServerName varchar(20), 
				@PartitionColumn int, 
				@IsDebug bit, 
				@return_value int OUTPUT', 
				@LoadTableName = @TableName, 
				@LinkedServerName = @LinkedServerName, 
				@PartitionColumn = @PartitionColumn,
				@IsDebug = @IsDebug, 
				@return_value = @ReturnVal OUTPUT
			
			-- SELECT	'Return Value' = @ReturnVal
			
			SELECT @StartTime = (@TempTime)
			SELECT @EndTime = (GETDATE())	
			IF @IsDebug = 0 PRINT '--Execution time for loading ' + @LoadTableName + ': ' + CONVERT(varchar(20),@EndTime - @StartTime, 114)
			
		  END
		
		IF @PostProcessingSprocName IS NOT NULL AND @PostProcessingSprocName NOT LIKE ''
		BEGIN
			SELECT @SQL_String = N'
				EXEC @return_value = 
				[dbo].[' + @PostProcessingSprocName + ']
					@LoadTableName,
					@ReferentialTableName,
					@LinkedServerName,
					@PartitionColumn,
					@IsDebug;
		'
			
			EXECUTE sp_executesql @SQL_String, N'
				@LoadTableName varchar(255), 
				@ReferentialTableName varchar(255), 
				@LinkedServerName varchar(20), 
				@PartitionColumn int, 
				@IsDebug bit, 
				@return_value int OUTPUT', 
				@LoadTableName = @LoadTableName, 
				@ReferentialTableName = @ReferentialTableName,
				@LinkedServerName = @LinkedServerName, 
				@PartitionColumn = @PartitionColumn,
				@IsDebug = @IsDebug, 
				@return_value = @ReturnVal OUTPUT
			
			-- SELECT	'Return Value' = @ReturnVal
		END
		
		IF @IsDebug = 0
			BEGIN
				SELECT @SQL_String = N'SELECT @Count = (SELECT count(*) FROM ' + @TableName + ')'
				EXECUTE sp_executesql @SQL_String, N'@Count int OUTPUT', @Count = @TableCount OUTPUT
			
				SELECT @TableName + ': ' + CONVERT(varchar(10), @TableCount) AS [Record Count] 
			END
			
		FETCH NEXT FROM LoadCursor INTO @LoadSprocName, @PreProcessingSprocName, @PostProcessingSprocName, @CreateTableSprocName, @TableName, @ReferentialTableName 
	END

	CLOSE LoadCursor
	DEALLOCATE LoadCursor
END