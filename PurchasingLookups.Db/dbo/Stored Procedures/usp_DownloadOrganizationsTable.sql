-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download organizational data and ultimately load into the vOrganizations table.
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
--	2012-07-10 by kjt: Changed to slower merge load as per Alan Lai in order to preserve records
--	missing from DaFIS.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadOrganizationsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vOrganizations', --Load table name, i.e. name of table being loaded. 
	@ReferentialTableName varchar(244) = 'vAccounts', --Name of table being referenced 
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
		 [Id]
		,ISNULL ([Name], [Id]) AS [Name]
		,[TypeCode]
		,[TypeName]
		,[ParentId]
		,(CASE [IsActive] WHEN ''Y'' THEN 1 ELSE 0 END) AS [IsActive]
      
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT DISTINCT
			O.CHART_NUM || ''''-'''' || O.ORG_ID AS Id,
			O.ORG_NAME AS Name,
			O.ORG_TYPE_CODE AS TypeCode,
			O.org_type_name AS TypeName,
			O.CHART_NUM || ''''-'''' || O.rpts_to_org_id AS ParentId,
			O.ACTIVE_IND AS IsActive
		FROM FINANCE.ORGANIZATION_HIERARCHY O
		WHERE O.FISCAL_YEAR = 9999 AND O.FISCAL_PERIOD = ''''--'''' 
	'')
) ' + @LinkedServerName + '_' + @LoadTableName + ' ON ' + @LoadTableName + '.Id = ' + @LinkedServerName + '_' + @LoadTableName + '.Id
WHEN MATCHED THEN UPDATE SET
	    ' + @LoadTableName + '.[Name] = ' + @LinkedServerName + '_' + @LoadTableName + '.[Name]
	   ,' + @LoadTableName + '.[TypeCode] = ' + @LinkedServerName + '_' + @LoadTableName + '.[TypeCode]
	   ,' + @LoadTableName + '.[TypeName] = ' + @LinkedServerName + '_' + @LoadTableName + '.[TypeName]
	   ,' + @LoadTableName + '.[ParentId] = ' + @LinkedServerName + '_' + @LoadTableName + '.[ParentId]
	   ,' + @LoadTableName + '.[IsActive] = ' + @LinkedServerName + '_' + @LoadTableName + '.[IsActive]
WHEN NOT MATCHED BY TARGET THEN INSERT VALUES 
 (
	   [Id]
      ,[Name]
      ,[TypeCode]
      ,[TypeName]
      ,[ParentId]
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