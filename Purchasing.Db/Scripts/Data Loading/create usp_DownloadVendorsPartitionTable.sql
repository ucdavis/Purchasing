USE [PrePurchasing]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download Vendors data and ultimately load into the vVendorsPartitionTable
-- Modifications: 
--	2012-03-21 by kjt: Added IsActive bit as per Alan Lai.
-- =============================================
ALTER PROCEDURE usp_DownloadVendorsPartitionTable
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vVendorsPartitionTable_Load', --Name of load table being loaded 
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
	
	TRUNCATE TABLE [PrePurchasing].[dbo].[' + @LoadTableName + ']
	
	INSERT INTO ' + @LoadTableName + ' 
	SELECT
		 [Id]
		,[Name]	
		,[OwnershipCode]
		,[BusinessTypeCode]
		,(CASE WHEN VendorInactiveDate <= GETDATE() THEN 0
		  ELSE 1 END) AS [IsActive]
	  , ' + Convert(char(1), @PartitionColumn) + ' AS [PartitionColumn]
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT
			vendor_id AS Id,
			vendor_name  AS Name,
			vendor_ownership_code  AS OwnershipCode,
			business_type_code AS BusinessTypeCode,
			vendor_inactive_date AS VendorInactiveDate
		FROM FINANCE.VENDOR
		WHERE vendor_name IS NOT NULL
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
GO
