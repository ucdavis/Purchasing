-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download Commodities data and ultimately load into the vCommodities table
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadCommoditiesTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vCommodities', --Name of table being loaded 
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
	
	INSERT INTO ' + @LoadTableName + ' 
	SELECT
		[Id]
		,[Name]	
		,[GroupCode]
		,[SubGroupCode]
		,(CASE [IsActive] WHEN ''Y'' THEN 1 ELSE 0 END) AS [IsActive]
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT
			commodity_num AS Id,
			commodity_desc AS Name,
			commodity_group_code AS GroupCode,
			commodity_sub_group_code AS SubGroupCode,
			active_ind  AS IsActive
		FROM FINANCE.COMMODITY
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