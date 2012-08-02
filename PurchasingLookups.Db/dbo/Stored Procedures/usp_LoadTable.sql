-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Call a table's load procedure to load a non-partitioned table.
-- Modifications:
--  2012-02-22 by kjt: Added to only print timing statements is @IsDebug = 0
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_LoadTable] 
	@LoadSprocName varchar(255) = 'usp_DownloadAccountsTable', --Name of sproc that loads data from Campus FIS database.
	@CreateTableSprocName varchar(255) = 'usp_CreateAccountsTable', --Name of sproc that that creates the empty table.
	@LoadTableName varchar(255) = 'vAccounts', --Can be passed another table name, i.e. vAccounts, etc.
	@ReferentialTableName varchar(255) = '', --Can be passed a referental table name, i.e. vAccounts, etc. Typically blank except when loading vOrganizations... OR vAccounts
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server.  This allows to use something other than 'FIS_DS' if needed. 
	@IsDebug bit = 0 -- Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	DECLARE @MyDate datetime = GetDate()
	
	DECLARE @TSQL varchar(MAX) = ''
		
	DECLARE @StartTime datetime = (SELECT GETDATE())
	DECLARE @TempTime datetime = (SELECT @StartTime)
	DECLARE @EndTime datetime = (SELECT @StartTime)
	IF @IsDebug = 0 PRINT '--Start time for loading ' + @LoadTableName + ': ' +  + CONVERT(varchar(20),@StartTime, 114)
	
	-- Create the main table if it does not already exist:
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[' + @LoadTableName + ']') AND type in (N'U'))
	BEGIN
		SELECT @TSQL = 'EXEC [dbo].[' + @CreateTableSprocName + '] 
		@LoadTableName = ' + @LoadTableName + ', '
		IF @ReferentialTableName IS NOT NULL AND @ReferentialTableName NOT LIKE ''
			SELECT @TSQL += '@ReferentialTableName = ' + @ReferentialTableName + ', ' 
		SELECT @TSQL += '
		@IsDebug = ' + convert(varchar(1), @IsDebug) + ''
	END
		
	SELECT @TSQL += '
	EXEC ' + @LoadSprocName + ' 
		@LoadTableName = ' + @LoadTableName + ', '
		IF @ReferentialTableName IS NOT NULL AND @ReferentialTableName NOT LIKE ''
			SELECT @TSQL += '@ReferentialTableName = ' + @ReferentialTableName + ', ' 
		SELECT @TSQL += '
		@LinkedServerName = ' + @LinkedServerName + ',  
		@IsDebug = ' + convert(varchar(1), @IsDebug) 
    IF @IsDebug = 1 
    BEGIN 
		EXEC (@TSQL) 
		SELECT @TSQL = ''
	END
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
	
	IF @IsDebug = 0 PRINT '--Execution time for loading ' + @LoadTableName + ': ' + CONVERT(varchar(20),@EndTime - @StartTime, 114)
	
END