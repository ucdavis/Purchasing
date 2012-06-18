USE [PrePurchasing]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download VendorAddresses data and ultimately load into the vVendorAddressesPartitionTable
-- =============================================
ALTER PROCEDURE usp_DownloadVendorAddressesPartitionTable
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vVendorAddressesPartitionTable_Load', --Name of load table being loaded 
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
	
	INSERT INTO ' + @LoadTableName + ' (
		[VendorId],
		[TypeCode],
		[Name],
		[Line1],
		[Line2],
		[Line3],
		[City],
		[State],
		[Zip],
		[CountryCode],
		[PartitionColumn])
	SELECT
		[VendorId],
		[TypeCode],
		[Name],
		[Line1],
		[Line2],
		[Line3],
		[City],
		[State],
		[Zip],
		[CountryCode],
       ' + Convert(char(1), @PartitionColumn) + ' AS [PartitionColumn]
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT
			vendor_id AS VendorId,
			address_type_code AS TypeCode,
			address_name  AS Name,
			address_line_1 Line1,
			address_line_2 Line2,
			address_line_3 Line3,
			city_name City,
			state_code State,
			zip_code Zip,
			country_code CountryCode 
		FROM FINANCE.VENDOR_ADDRESS
		WHERE address_name IS NOT NULL
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
