
CREATE PROCEDURE [dbo].[usp_Update_vOrganizations_From_vOrganizationsSync]
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

	INSERT INTO [dbo].[vOrganizations] (
	   [Id]
      ,[Name]
      ,[TypeCode]
      ,[TypeName]
      ,[ParentId]
      ,[IsActive]
      ,[UpdateHash]
	)
	SELECT [Id]
      ,[Name]
      ,[TypeCode]
      ,[TypeName]
      ,[ParentId]
      ,[IsActive]
      ,[UpdateHash]
	FROM [dbo].[vOrganizations_sync]
	WHERE [ID] NOT IN (SELECT [Id] FROM [dbo].[vOrganizations])

	UPDATE [dbo].[vOrganizations]
	SET    [Name] = t2.[Name]
		  ,[TypeCode] = t2.[TypeCode]
		  ,[TypeName] = t2.[TypeName]
		  ,[ParentId] = t2.[ParentId]
		  ,[IsActive] = t2.[IsActive]
	   ,[UpdateHash] = t2.[UpdateHash]
	FROM  [dbo].[vOrganizations] t1
	INNER JOIN [dbo].[vOrganizations_sync]  t2 ON t1.Id = t2.Id

	TRUNCATE TABLE  [dbo].[vOrganizations_sync]
END