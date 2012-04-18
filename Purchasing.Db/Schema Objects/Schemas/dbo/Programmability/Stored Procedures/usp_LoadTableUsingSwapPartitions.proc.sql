-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Call a table's load procedure to load a partitioned "load" table 
-- and then swap the entire partition to the main table.
-- Modifications:
-- 2012-02-22 by kjt: Added to only print timing statements is @IsDebug = 0
-- =============================================
CREATE PROCEDURE [dbo].[usp_LoadTableUsingSwapPartitions] 
	@LoadSprocName varchar(255) = 'usp_DownloadAccountsPartitionTable', --Name of sproc that loads data from Campus FIS database.
	@CreateTableSprocName varchar(255) = 'usp_CreateAccountsPartitionTable', --Name of sproc that that creates the load and empty partition tables.
	@TableName varchar(255) = 'vAccountsPartitionTable', --Can be passed another table name, i.e. #vAccountsPartitionTable, etc.
	@ReferentialTableName varchar(255) = '', --Can be passed a referental table name, i.e. vAccountsPartitionTable, etc. Typically blank except when loading vOrganizations... OR vAccounts
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server.  This allows to use something other than 'FIS_DS' if needed. 
	@IsDebug bit = 0 -- Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	DECLARE @MyDate datetime = GetDate()
	
	DECLARE @DestinationTableName varchar(255) = @TableName 
	
	DECLARE @LoadTableName varchar(255) = @DestinationTableName + '_Load'
	
	DECLARE @ReferentialLoadTableName varchar(255) = @ReferentialTableName + '_Load'

	DECLARE @EmptyTableName varchar(255) = @DestinationTableName + '_Empty'
	
	DECLARE @PartnNum smallint = (select dbo.udf_GetEvenOddPartitionNumber(@MyDate))
	DECLARE @OldPartnNum smallint = (SELECT dbo.udf_GetEvenOddPartitionNumber(DATEADD(d, -1, @MyDate)))

	
	DECLARE @TSQL varchar(MAX) = ''
		
	DECLARE @StartTime datetime = (SELECT GETDATE())
	DECLARE @TempTime datetime = (SELECT @StartTime)
	DECLARE @EndTime datetime = (SELECT @StartTime)
	IF @IsDebug = 0 PRINT '--Start time for loading ' + @LoadTableName + ' using swap partitions: ' +  + CONVERT(varchar(20),@StartTime, 114)
	
	-- Create the main table if it does not already exist:
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']') AND type in (N'U'))
	BEGIN
		SELECT @TSQL = 'EXEC [dbo].[' + @CreateTableSprocName + '] 
		@TableNamePrefix = ' + @TableName + ', '
		IF @ReferentialTableName IS NOT NULL AND @ReferentialTableName NOT LIKE ''
			SELECT @TSQL += '@ReferentialTableNamePrefix = ' + @ReferentialTableName + ', ' 
		SELECT @TSQL += '
		@Mode = 1, 
		@IsDebug = ' + convert(varchar(1), @IsDebug) + ''	
	END
	
	SELECT @TSQL += '
	EXEC [dbo].[' + @CreateTableSprocName + '] 
		@TableNamePrefix = ' + @TableName + ', '
		IF @ReferentialTableName IS NOT NULL AND @ReferentialTableName NOT LIKE ''
			SELECT @TSQL += '@ReferentialTableNamePrefix = ' + @ReferentialTableName + ', ' 
	SELECT @TSQL += '
		@Mode = 2, 
		@IsDebug = ' + convert(varchar(1), @IsDebug) + '
		
	EXEC ' + @LoadSprocName + ' 
		@LoadTableName = ' + @LoadTableName + ', '
		IF @ReferentialTableName IS NOT NULL AND @ReferentialTableName NOT LIKE ''
			SELECT @TSQL += '@ReferentialTableName = ' + @ReferentialTableName + ', ' 
		SELECT @TSQL += '
		@LinkedServerName = ' + @LinkedServerName + ', 
		@PartitionColumn = ' + convert(char(1),@PartnNum) + ', 
		@IsDebug = ' + convert(varchar(1), @IsDebug) 
    IF @IsDebug = 1 
    BEGIN 
		EXEC (@TSQL) 
		SELECT @TSQL = ''
	END
	PRINT '--' + @TableName + '''s new partition number is ' + CONVERT(char(1),@PartnNum)
	
		-- Note that the receiving partition MUST be empty.
		-- Therefore, 
		-- Swap the tables's old partition's record set with an empty one from Tablename's _Empty table
		SELECT @TSQL += '
	ALTER TABLE ' + @DestinationTableName + 
'
	SWITCH PARTITION ' + CONVERT(char(1),@PartnNum) + ' TO ' + @EmptyTableName + ' PARTITION ' + CONVERT(char(1),@PartnNum)
	SELECT @TSQL += '
	ALTER TABLE ' + @DestinationTableName + 
'
	SWITCH PARTITION ' + CONVERT(char(1),@OldPartnNum) + ' TO ' + @EmptyTableName + ' PARTITION ' + CONVERT(char(1),@OldPartnNum)
	 
		-- Swap the Tablename's _Load partition's record set with the newly empty one from Tablename
		SELECT @TSQL += '
	ALTER TABLE ' + @LoadTableName + 
'
	SWITCH PARTITION ' + CONVERT(char(1),@PartnNum) + ' TO ' + @DestinationTableName + ' PARTITION ' + CONVERT(char(1),@PartnNum)
		 
	SELECT @TSQL += '
	TRUNCATE TABLE ' + @EmptyTableName
	
	SELECT @TSQL += '
	DROP TABLE ' + @LoadTableName + '
	DROP TABLE ' + @EmptyTableName
	
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

	SELECT @StartTime = (@TempTime)
	
	SELECT @EndTime = (GETDATE())
	
	
	IF @IsDebug = 0 PRINT '--Execution time for loading ' + @LoadTableName + ' using swap partitions: ' + CONVERT(varchar(20),@EndTime - @StartTime, 114)
	
END