-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Download VendorAddresses data and ultimately load into the vVendorAddresses Table
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
--	Also added IsDefault (bit) column as per Alan Lai
-- =============================================
CREATE PROCEDURE [dbo].[usp_DownloadVendorAddressesTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vVendorAddresses', --Name of table being loaded 
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
		[PhoneNumber],
		[FaxNumber],
		[Email], [Url],
		[IsDefault] --(vendor address)
		)
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
		[PhoneNumber], [FaxNumber],
		[Email_ID], [Web_Id],
		[Is_Default] --(vendor order address)
	FROM 
	OPENQUERY(' + @LinkedServerName + ', ''
		SELECT
			FINANCE.VENDOR_ADDRESS.vendor_id AS VendorId,
			address_type_code AS TypeCode,
			address_name  AS Name,
			address_line_1 Line1,
			address_line_2 Line2,
			address_line_3 Line3,
			city_name City,
			state_code State,
			zip_code Zip,
			country_code CountryCode ,
			phone_number PhoneNumber,
			fax_number FaxNumber,
			email_id, web_id,
			(CASE WHEN ADDRESS_TYPE_CODE = DEFAULT_ORDER_ADDR_TYPE_CODE THEN 1
			      ELSE 0 
			 END) AS IS_DEFAULT
		FROM FINANCE.VENDOR_ADDRESS
		LEFT OUTER JOIN FINANCE.VENDOR ON FINANCE.VENDOR.VENDOR_ID = FINANCE.VENDOR_ADDRESS.VENDOR_ID
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