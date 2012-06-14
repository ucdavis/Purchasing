-- =============================================
-- Author:		Ken Taylor
-- Create date: February 22, 2012
-- Description:	Download UnitOfMeasure data and ultimately load into the UnitOfMeasures table
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadUnitOfMeasuresTable_bak]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'UnitOfMeasures', --Name of table being loaded 
	@LinkedServerName varchar(20) = 'FIS_DS', --Name of the linked DaFIS server.
	@PartitionColumn char(1) = 0, --Number to use for partition column, N/A in this case
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TSQL varchar(MAX) = ''

    -- Insert statements for procedure here
	SELECT @TSQL = '
    
merge ' + @LoadTableName + '  as ' + @LoadTableName + ' 
using
(
	SELECT
		 [Id]
		,[Name]	
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT
			unit_of_measure_code AS Id,
			unit_of_measure_name AS Name
			
		FROM FINANCE.AR_UNIT_OF_MEASURE
	'')
) ' + @LinkedServerName + '_' + @LoadTableName + ' ON ' + @LoadTableName + '.Id = ' + @LinkedServerName + '_' + @LoadTableName + '.Id

WHEN MATCHED THEN UPDATE set
	' + @LoadTableName + '.Name = ' + @LinkedServerName + '_' + @LoadTableName + '.Name
WHEN NOT MATCHED BY TARGET THEN INSERT VALUES 
 (
	 Id
	,Name
 )
--WHEN NOT MATCHED BY SOURCE THEN DELETE
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