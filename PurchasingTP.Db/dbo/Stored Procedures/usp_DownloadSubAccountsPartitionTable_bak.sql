-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download sub-accounts data and ultimately load into the vSubAccountsPartitionTable
-- =============================================
CREATE PROCEDURE [usp_DownloadSubAccountsPartitionTable_bak]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vSubAccountsPartitionTable_Load', --Name of load table being loaded 
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server.
	@PartitionColumn char(1) = 0, --Number to use for partition column
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TSQL varchar(MAX) = ''

    -- Insert statements for procedure here
	SELECT @TSQL = '
	
	TRUNCATE TABLE [PrePurchasing].[dbo].[' + @LoadTableName + ']
	
	INSERT INTO ' + @LoadTableName + ' (AccountNumber, SubAccountNumber, Name, IsActive, PartitionColumn)
	SELECT DISTINCT
		[AccountNumber]
		,[SubAccountNumber]
		,[Name]
		,(CASE [IsActive] WHEN ''Y'' THEN 1 ELSE 0 END) AS [IsActive]
      , ' + Convert(char(1), @PartitionColumn) + ' AS [PartitionColumn]
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT DISTINCT
			S.CHART_NUM || ''''-'''' || S.acct_num AS AccountNumber,
			S.sub_acct_num AS SubAccountNumber,
			S.sub_acct_name  AS Name,
			S.sub_acct_active_ind  AS IsActive
		FROM FINANCE.SUB_ACCOUNT S
		WHERE S.FISCAL_YEAR = 9999 AND S.FISCAL_PERIOD = ''''--'''' 
	'')'
	
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