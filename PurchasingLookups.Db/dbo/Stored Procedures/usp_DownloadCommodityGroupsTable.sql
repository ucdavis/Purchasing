-- =============================================
-- Author:		Ken Taylor
-- Create date: May 31, 2012
-- Description:	Download CommodityGroups data and ultimately load into the vCommodityGroups table.
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadCommodityGroupsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vCommodityGroups', --Name of table being loaded 
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
	
	INSERT INTO ' + @LoadTableName + ' ([GroupCode]
		,[Name]	
		,[SubGroupCode]
		,[SubGroupName]
		)
	SELECT
		 [GroupCode]
		,[Name]	
		,[SubGroupCode]
		,[SubGroupName]		
      
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT
			COMMODITY_GROUP_CODE AS GroupCode,
			COMMODITY_GROUP_DESC AS Name,
			COMMODITY_SUB_GROUP_CODE AS SubGroupCode,
			COMMODITY_SUB_GROUP_DESC AS SubGroupName
		FROM FINANCE.COMMODITY_GROUP
		ORDER BY COMMODITY_GROUP_CODE, COMMODITY_SUB_GROUP_CODE
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