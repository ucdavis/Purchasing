-- =============================================
-- Author:		Ken Taylor
-- Create date: February 22, 2012
-- Description:	Download Campus Buildings data and ultimately load into the vBuildings table.
-- Modifications: 2012-03-12 by kjt: Added Id field as per Alan Lai.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadBuildingsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vBuildings', --Name of table being loaded 
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
		 campus_code + ''-'' + building_code as [Id]
		,campus_code AS [CampusCode]
		,building_code AS [BuildingCode]
		,campus_name AS [CampusName]
		,campus_short_name AS [CampusShortName]
		,campus_type_code AS [CampusTypeCode]
		,building_name AS [BuildingName]
		,ds_last_update_date AS [LastUpdateDate]
		,(CASE active_ind WHEN ''Y'' THEN 1
						  WHEN ''N'' THEN 0
	      END) AS [IsActive]
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT
			campus_code,
			building_code,
			campus_name,
			campus_short_name,
			campus_type_code,
			building_name,
			ds_last_update_date,
			active_ind 			
		FROM FINANCE.CAMPUS_BUILDING
	'')
) ' + @LinkedServerName + '_' + @LoadTableName + ' ON ' + @LoadTableName + '.CampusCode   = ' + @LinkedServerName + '_' + @LoadTableName + '.CampusCode AND
					   ' + @LoadTableName + '.BuildingCode = ' + @LinkedServerName + '_' + @LoadTableName + '.BuildingCode

WHEN MATCHED THEN UPDATE set
	' + @LoadTableName + '.[CampusName] = ' + @LinkedServerName + '_' + @LoadTableName + '.[CampusName],
	' + @LoadTableName + '.[CampusShortName] = ' + @LinkedServerName + '_' + @LoadTableName + '.[CampusShortName],
	' + @LoadTableName + '.[CampusTypeCode] = ' + @LinkedServerName + '_' + @LoadTableName + '.[CampusTypeCode],
	' + @LoadTableName + '.[BuildingName] = ' + @LinkedServerName + '_' + @LoadTableName + '.[BuildingName],
	' + @LoadTableName + '.[LastUpdateDate] = ' + @LinkedServerName + '_' + @LoadTableName + '.[LastUpdateDate],
	' + @LoadTableName + '.[IsActive] = ' + @LinkedServerName + '_' + @LoadTableName + '.[IsActive]

WHEN NOT MATCHED BY TARGET THEN INSERT VALUES 
 (	 [Id]
	,[CampusCode]
	,[BuildingCode]
	,[CampusName]
	,[CampusShortName]
	,[CampusTypeCode]
	,[BuildingName]
	,[LastUpdateDate]
	,[IsActive]
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