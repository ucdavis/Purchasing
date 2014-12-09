
CREATE PROCEDURE [dbo].[usp_Update_vVendorAddresses_From_vVendorAddressesSync]
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

   MERGE [dbo].[vVendorAddresses] t1
   USING (
		SELECT [Id]
		  ,[VendorId]
		  ,[TypeCode]
		  ,ISNULL([DetailId], '0') [DetailId]
		  ,[Name]
		  ,[Line1]
		  ,[Line2]
		  ,[Line3]
		  ,[City]
		  ,[State]
		  ,[Zip]
		  ,[CountryCode]
		  ,[PhoneNumber]
		  ,[FaxNumber]
		  ,[Email]
		  ,[Url]
		  ,[IsDefault]
		  ,[IsActive]
		  ,[UpdateHash]
  FROM [dbo].[vVendorAddresses_sync]
   ) t2 ON t1.[VendorAddressId] = t2.[Id]

   WHEN MATCHED THEN UPDATE SET
	     [VendorId] = t2.[VendorId]
		,[TypeCode] = t2.[TypeCode]
		,[DetailId] = t2.[DetailId]
		,[VendorAddressId] = t2.[Id]
		,[Name] = t2.[Name]
		,[Line1] = t2.[Line1]
		,[Line2] = t2.[Line2]
		,[Line3] = t2.[Line3]
		,[City] = t2.[City]
		,[State] = t2.[State]
		,[Zip] = t2.[Zip]
		,[CountryCode] = t2.[CountryCode]
		,[PhoneNumber] = t2.[PhoneNumber]
		,[FaxNumber] = t2.[FaxNumber]
		,[Email] = t2.[Email]
		,[Url] = t2.[Url]
		,[IsDefault] = t2.[IsDefault]
		,[IsActive] = t2.[IsActive]
		,[UpdateHash] = t2.[UpdateHash]

	WHEN NOT MATCHED BY TARGET THEN INSERT (
	   [VendorId]
      ,[TypeCode]
	  ,[DetailId]
	  ,[VendorAddressId]
      ,[Name]
      ,[Line1]
      ,[Line2]
      ,[Line3]
      ,[City]
      ,[State]
      ,[Zip]
      ,[CountryCode]
      ,[PhoneNumber]
      ,[FaxNumber]
      ,[Email]
      ,[Url]
      ,[IsDefault]
      ,[IsActive]
      ,[UpdateHash]
	)
	VALUES (
	   [VendorId]
      ,[TypeCode]
	  ,[DetailId]
	  ,[Id]
      ,[Name]
      ,[Line1]
      ,[Line2]
      ,[Line3]
      ,[City]
      ,[State]
      ,[Zip]
      ,[CountryCode]
      ,[PhoneNumber]
      ,[FaxNumber]
      ,[Email]
      ,[Url]
      ,[IsDefault]
      ,[IsActive]
      ,[UpdateHash]
	)

	WHEN NOT MATCHED BY SOURCE THEN UPDATE SET
		t1.[IsActive] = 0
	;

   --TRUNCATE TABLE [dbo].[vVendorAddresses_sync]
END