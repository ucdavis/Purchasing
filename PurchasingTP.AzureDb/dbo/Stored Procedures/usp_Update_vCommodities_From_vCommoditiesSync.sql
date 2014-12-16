

CREATE PROCEDURE [dbo].[usp_Update_vCommodities_From_vCommoditiesSync]
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

	 -- INSERT ant new records:
	 INSERT INTO [dbo].[vCommodities] ( 
		   [Id]
		  ,[Name]
		  ,[GroupCode]
		  ,[SubGroupCode]
		  ,[IsActive] 
		  ,[UpdateHash] 
	 )
	SELECT [Id]
		  ,[Name]
		  ,[GroupCode]
		  ,[SubGroupCode]
		  ,[IsActive] 
		  ,[UpdateHash] 
	FROM [dbo].[vCommodities_sync]
	WHERE [Id] NOT IN (SELECT [Id] FROM [dbo].[vCommodities])

	-- UPDATE any existing records:
	   UPDATE [dbo].[vCommodities]
	SET    [Name] = t2.[Name]
		  ,[GroupCode] = t2.[GroupCode] 
		  ,[SubGroupCode] = t2.[SubGroupCode]
		  ,[IsActive] = t2.[IsActive]
		  ,[UpdateHash] = t2.[UpdateHash]
	FROM [dbo].[vCommodities] t1
	INNER JOIN [dbo].[vCommodities_sync]  t2 ON t1.Id = t2.Id

	-- DELETE records from the sync table when finished:
	--TRUNCATE TABLE [dbo].[vCommodities_sync]
END