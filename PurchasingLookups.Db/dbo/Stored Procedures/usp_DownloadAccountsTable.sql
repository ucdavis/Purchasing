﻿-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download account data and ultimately load into the vAccounts table.
-- Modifications:
--	2012-02-16 by kjt: Removed post-loading Organizations FK creation logic and made its
-- own separate sproc because that portion would fail if accounts table contained the prior day's data in
-- the other partition.
--	2012-05-31 by kjt: Added inner join logic to organizations to keep accounts with 
-- bogus orgs from being loaded as this causes the vAccounts-vOrganizations FK constraint to
-- error out.
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadAccountsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vAccounts table', --Name of table being loaded. 
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
	TRUNCATE TABLE [PrePurchasingLookups].[dbo].[' + @LoadTableName + ']
	
	INSERT INTO ' + @LoadTableName + ' SELECT 
       [Id]
      ,[Name]
      ,  (CASE WHEN (ExpirationDate IS NOT NULL AND ExpirationDate <= GETDATE()) THEN 0
		  ELSE 1 END) AS [IsActive]
      ,[AccountManager]
      ,[AccountManagerId]
      ,[PrincipalInvestigator]
      ,[PrincipalInvestigatorId]
      ,[OrganizationId]
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT 
			A.CHART_NUM || ''''-'''' || A.ACCT_NUM AS Id,
			A.ACCT_NAME AS Name,
			A.acct_expiration_date AS ExpirationDate,
			A.ACCT_MGR_NAME AS AccountManager,
			A.ACCT_MGR_ID AS AccountManagerId,
			A.PRINCIPAL_INVESTIGATOR_NAME AS PrincipalInvestigator,
			A.PRINCIPAL_INVESTIGATOR_ID AS PrincipalInvestigatorId,
			A.CHART_NUM || ''''-'''' || A.ORG_ID AS OrganizationId
		FROM FINANCE.ORGANIZATION_ACCOUNT A
		INNER JOIN FINANCE.ORGANIZATION_HIERARCHY O
		ON A.CHART_NUM = O.CHART_NUM AND A.ORG_ID = O.ORG_ID
		WHERE A.FISCAL_YEAR = 9999 AND A.FISCAL_PERIOD = ''''--'''' 
		AND A.ORG_ID IS NOT NULL
		AND O.FISCAL_YEAR = 9999 AND O.FISCAL_PERIOD = ''''--'''' 
	'')
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