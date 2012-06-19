-- =============================================
-- Author:		Ken Taylor
-- Create date: May 31, 2012
-- Description:	Download CommodityGroups data and ultimately load into the vCommodityGroupsPartitionTable
-- =============================================
create PROCEDURE [dbo].[usp_DownloadCommodityGroupsPartitionTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vCommodityGroupsPartitionTable_Load', --Name of load table being loaded 
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
	
	TRUNCATE TABLE [PrePurchasingLookups].[dbo].[' + @LoadTableName + ']
	
	INSERT INTO ' + @LoadTableName + ' ([GroupCode]
		,[Name]	
		,[SubGroupCode]
		,[SubGroupName]
		,[PartitionColumn]	
		)
	SELECT
		 [GroupCode]
		,[Name]	
		,[SubGroupCode]
		,[SubGroupName]		
      , ' + Convert(char(1), @PartitionColumn) + ' AS [PartitionColumn]
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