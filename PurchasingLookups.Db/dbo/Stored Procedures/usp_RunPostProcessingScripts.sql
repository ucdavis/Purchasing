-- =============================================
-- Author:		Ken Taylor
-- Create date: June 26, 2012
-- Description:	Run all of the new PrePurchasingLookups post-processing scripts as per Alan Lai
-- Note: 
--		Comment out specific rows in @ParamsTable table to alter which tables are loaded.
--		Change TableName and ReferentialTableName fields in @ParamsTable table to alter names of tables being loaded.
--
-- Usage:
--	USE [PrePurchasingLookups]
--	GO
--
--	DECLARE	@return_value int
-- 
--	EXEC	@return_value = [dbo].[usp_RunPostProcessingScripts] [@LinkedServerName = <some_other_linked_server>] [@IsDebug = 1]
-- 
--	SELECT	'Return Value' = @return_value
-- 
--	GO
--
-- Modifications:

-- =============================================
CREATE PROCEDURE [dbo].[usp_RunPostProcessingScripts] 
	-- Add the parameters for the stored procedure here
	@LinkedServerName varchar(20) = '', --Can be changed to whatever the 'full' access linked server connection is. 
	@IsDebug bit = 0 --Set to 1 to display SQL statements
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/*
	DECLARE @LinkedServerName varchar(10) = 'FIS_DS'
    DECLARE @IsDebug bit = 0
*/
	DECLARE	@ReturnVal int
	DECLARE @TableCount int = 0
	DECLARE @SQL_String nvarchar(MAX) = N''
	
	SELECT @LinkedServerName = (SELECT dbo.udf_GetParameterValue(@LinkedServerName, 'FIS_DS', 'LinkedServerName'))

	DECLARE @ParamsTable TABLE (PostProcessingSprocName varchar (255), TableName varchar(255), ReferentialTableName varchar(255))
	
	INSERT INTO @ParamsTable VALUES 
		 ('usp_Post_DownloadAccountsTable_Processing', (SELECT dbo.udf_GetParameterValue(NULL, 'vAccounts', 'AccountsTableName')), (SELECT dbo.udf_GetParameterValue(NULL, 'vOrganizations', 'OrganizationsTableName')))
		,('usp_Post_DownloadCommoditiesTable_Processing', (SELECT dbo.udf_GetParameterValue(NULL, 'vCommodities', 'CommoditiesTableName')), '')
		,('usp_Post_DownloadVendorsTable_Processing', (SELECT dbo.udf_GetParameterValue(NULL, 'vVendors', 'VendorsTableName')), '')
		,('usp_Post_DownloadBuildingsTable_Processing', (SELECT dbo.udf_GetParameterValue(NULL, 'vBuildings', 'BuildingsTableName')), '')
		
	IF @IsDebug = 1 
	BEGIN
		SELECT 'Linked Server Name: ' + @LinkedServerName
		SELECT * FROM @ParamsTable
	END
	
	DECLARE @PostProcessingSprocName varchar (255), @TableName varchar(255), @ReferentialTableName varchar(255)
		
	DECLARE PostProcessingCursor CURSOR FOR SELECT * FROM @ParamsTable
	OPEN PostProcessingCursor 
	FETCH NEXT FROM PostProcessingCursor INTO @PostProcessingSprocName, @TableName, @ReferentialTableName 
	WHILE @@FETCH_STATUS <> -1
	BEGIN		
	
		IF @PostProcessingSprocName IS NOT NULL AND @PostProcessingSprocName NOT LIKE ''
		BEGIN
			SELECT @SQL_String = N'
				EXEC @return_value = 
				[dbo].[' + @PostProcessingSprocName + ']
					@TableName,
					@ReferentialTableName,
					@LinkedServerName,
					@IsDebug;
		'
			
			EXECUTE sp_executesql @SQL_String, N'
				@TableName varchar(255), 
				@ReferentialTableName varchar(255), 
				@LinkedServerName varchar(20), 
				@IsDebug bit, 
				@return_value int OUTPUT', 
				@TableName = @TableName, 
				@ReferentialTableName = @ReferentialTableName,
				@LinkedServerName = @LinkedServerName, 
				@IsDebug = @IsDebug, 
				@return_value = @ReturnVal OUTPUT
			
			-- SELECT	'Return Value' = @ReturnVal
		END
			
		FETCH NEXT FROM PostProcessingCursor INTO @PostProcessingSprocName, @TableName, @ReferentialTableName  
	END

	CLOSE PostProcessingCursor
	DEALLOCATE PostProcessingCursor
END
GO

