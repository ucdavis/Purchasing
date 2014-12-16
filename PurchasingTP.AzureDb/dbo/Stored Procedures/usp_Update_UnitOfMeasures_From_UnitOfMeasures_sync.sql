


CREATE PROCEDURE [dbo].[usp_Update_UnitOfMeasures_From_UnitOfMeasures_sync]
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

	INSERT INTO [dbo].[UnitOfMeasures] (
	   [Id]
      ,[Name]
	)
	SELECT [Id]
      ,[Name]
	FROM [dbo].[UnitOfMeasures_sync]
	WHERE [Id] NOT IN (SELECT [Id] FROM [dbo].[UnitOfMeasures])

	UPDATE [dbo].[UnitOfMeasures]
	SET    [Name] = t2.[Name]
	FROM [dbo].[UnitOfMeasures] t1
	INNER JOIN [dbo].[UnitOfMeasures_sync]  t2 ON t1.[Id] = t2.[Id]

	--TRUNCATE TABLE [dbo].[UnitOfMeasures_sync]
	END