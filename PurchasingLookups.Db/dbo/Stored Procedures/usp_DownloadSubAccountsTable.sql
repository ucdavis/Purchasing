-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download sub-accounts data and ultimately load into the vSubAccounts table
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
--	2012-07-10 by kjt: Changed to slower merge load as per Alan Lai in order to preserve records
--	missing from DaFIS.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadSubAccountsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vSubAccounts', --Name of table being loaded 
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
	
merge ' + @LoadTableName + ' as ' + @LoadTableName + ' 
using
(
	SELECT DISTINCT
		 [AccountNumber]
		,[SubAccountNumber]
		,[Name]
		,(CASE [IsActive] WHEN ''Y'' THEN 1 ELSE 0 END) AS [IsActive]
		
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT DISTINCT
			S.CHART_NUM || ''''-'''' || S.acct_num AS AccountNumber,
			S.sub_acct_num AS SubAccountNumber,
			S.sub_acct_name  AS Name,
			S.sub_acct_active_ind  AS IsActive
		FROM FINANCE.SUB_ACCOUNT S
		WHERE S.FISCAL_YEAR = 9999 AND S.FISCAL_PERIOD = ''''--'''' 
	'')
) ' + @LinkedServerName + '_' + @LoadTableName + ' ON ' + @LoadTableName + '.AccountNumber = ' + @LinkedServerName + '_' + @LoadTableName + '.AccountNumber AND
						  ' + @LoadTableName + '.SubAccountNumber = ' + @LinkedServerName + '_' + @LoadTableName + '.SubAccountNumber
WHEN MATCHED THEN UPDATE SET
	    ' + @LoadTableName + '.[Name] = ' + @LinkedServerName + '_' + @LoadTableName + '.[Name]	
	   ,' + @LoadTableName + '.[IsActive] = ' + @LinkedServerName + '_' + @LoadTableName + '.[IsActive]
WHEN NOT MATCHED BY TARGET THEN INSERT
	( 
	   [AccountNumber]
      ,[SubAccountNumber]
      ,[Name]
      ,[IsActive]
	) 
	VALUES 
	(
	   [AccountNumber]
      ,[SubAccountNumber]
      ,[Name]
      ,[IsActive]
	)
WHEN NOT MATCHED BY SOURCE THEN UPDATE SET
	   ' + @LoadTableName + '.[IsActive] = 0
 ;'
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