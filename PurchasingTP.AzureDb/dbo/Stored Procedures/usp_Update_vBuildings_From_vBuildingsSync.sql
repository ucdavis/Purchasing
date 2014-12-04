
CREATE PROCEDURE [dbo].[usp_Update_vBuildings_From_vBuildingsSync]
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

	INSERT INTO [dbo].[vBuildings] (
	   [Id]
      ,[CampusCode]
      ,[BuildingCode]
      ,[CampusName]
      ,[CampusShortName]
      ,[CampusTypeCode]
      ,[BuildingName]
      ,[LastUpdateDate]
      ,[IsActive]
      ,[UpdateHash]
	)
	SELECT [Id]
      ,[CampusCode]
      ,[BuildingCode]
      ,[CampusName]
      ,[CampusShortName]
      ,[CampusTypeCode]
      ,[BuildingName]
      ,[LastUpdateDate]
      ,[IsActive]
      ,[UpdateHash]
	FROM [dbo].[vBuildings_sync]
	WHERE [Id] NOT IN (SELECT [Id] FROM [dbo].[vBuildings])

	UPDATE [dbo].[vBuildings]
	SET    [CampusCode] = t2.[CampusCode]
		  ,[BuildingCode] = t2.[BuildingCode]
		  ,[CampusName] = t2.[CampusName]
		  ,[CampusShortName] = t2.[CampusShortName]
		  ,[CampusTypeCode] = t2.[CampusTypeCode]
		  ,[BuildingName] = t2.[BuildingName]
		  ,[LastUpdateDate] = t2.[LastUpdateDate]
		  ,[IsActive] = t2.[IsActive]
		  ,[UpdateHash] = t2.[UpdateHash]
	FROM [dbo].[vBuildings] t1
	INNER JOIN [dbo].[vBuildings_sync]  t2 ON t1.Id = t2.Id

	--TRUNCATE TABLE [dbo].[vBuildings_sync]
END