
CREATE PROCEDURE [dbo].[usp_Update_vAccounts_From_vAccountsSync]
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

	INSERT INTO [dbo].[vAccounts] (
	   [Id]
      ,[Name]
      ,[IsActive]
      ,[AccountManager]
      ,[AccountManagerId]
      ,[PrincipalInvestigator]
      ,[PrincipalInvestigatorId]
      ,[OrganizationId]
      ,[UpdateHash]
	)
	SELECT [Id]
      ,[Name]
      ,[IsActive]
      ,[AccountManager]
      ,[AccountManagerId]
      ,[PrincipalInvestigator]
      ,[PrincipalInvestigatorId]
      ,[OrganizationId]
      ,[UpdateHash]
	FROM [dbo].[vAccounts_sync]
	WHERE [Id] NOT IN (SELECT [Id] FROM [dbo].[vAccounts])

	UPDATE [dbo].[vAccounts]
	SET    [Name] = t2.[Name]
		  ,[IsActive] = t2.[IsActive]
		  ,[AccountManager] = t2.[AccountManager]
		  ,[AccountManagerId] = t2.[AccountManagerId]
		  ,[PrincipalInvestigator] = t2.[PrincipalInvestigator]
		  ,[PrincipalInvestigatorId] = t2.[PrincipalInvestigatorId]
		  ,[OrganizationId] = t2.[OrganizationId]
	   ,[UpdateHash] = t2.[UpdateHash]
	FROM [dbo].[vAccounts] t1
	INNER JOIN [dbo].[vAccounts_sync]  t2 ON t1.Id = t2.Id

	--TRUNCATE TABLE [dbo].[vAccounts_sync]
	END