-- =============================================
-- Author:		Ken Taylor
-- Create date: February 29, 2012
-- Description:	Delete Commodities index prior to downloading Commodities data.
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_Pre_DownloadCommoditiesTable_Processing]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vCommodities', --Name of table being loaded 
	@ReferentialTableName varchar(244) = '', -- N/A in this case 
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server. -- N/A in this case
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TSQL varchar(MAX) = ''

    -- Insert statements for procedure here
	SELECT @TSQL = '
	--Drop the full-text index:
	IF  EXISTS (
	SELECT * FROM sys.objects O 
	INNER JOIN sys.fulltext_indexes FTI ON O.object_id = FTI.object_id
	WHERE O.object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND O.type in (N''U'')
	)
	DROP FULLTEXT INDEX ON [dbo].[' + @LoadTableName + ']
	
	--Remove the table''s unique index:
	IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND name = N''' + @LoadTableName + '_Id_UDX'')
		DROP INDEX [' + @LoadTableName + '_Id_UDX] ON [dbo].[' + @LoadTableName + '] WITH ( ONLINE = OFF )
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