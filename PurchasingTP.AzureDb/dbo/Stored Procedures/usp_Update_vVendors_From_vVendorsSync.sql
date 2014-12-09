
CREATE PROCEDURE [dbo].[usp_Update_vVendors_From_vVendorsSync]
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

	INSERT INTO  [dbo].[vVendors] (
	   [Id]
      ,[Name]
      ,[OwnershipCode]
      ,[BusinessTypeCode]
      ,[IsActive]
      ,[UpdateHash]
	)
	SELECT [Id]
      ,[Name]
      ,[OwnershipCode]
      ,[BusinessTypeCode]
      ,[IsActive]
      ,[UpdateHash]
	FROM [dbo].[vVendors_sync]
	WHERE [Id] NOT IN (SELECT [Id] FROM [dbo].[vVendors])

	UPDATE [dbo].[vVendors]
	SET [Id] = t2.[Id]
		,[Name] = t2.[Name]
		,[OwnershipCode] = t2.[OwnershipCode]
		,[BusinessTypeCode] = t2.[BusinessTypeCode]
		,[IsActive] = t2.[IsActive]
		,[UpdateHash] = t2.[UpdateHash]
	FROM [dbo].[vVendors] t1
	INNER JOIN [dbo].[vVendors_sync]  t2 ON t1.[Id] = t2.[Id] 

	--TRUNCATE TABLE [dbo].[vVendors_sync]
END