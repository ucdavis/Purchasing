
CREATE PROCEDURE [dbo].[usp_Update_vSubAccounts_From_vSubAccountsSync]
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

	INSERT INTO [dbo].[vSubAccounts] (
	   [id]
      ,[AccountNumber]
      ,[SubAccountNumber]
      ,[Name]
      ,[IsActive]
      ,[UpdateHash]
	)
	SELECT [id]
      ,[AccountNumber]
      ,[SubAccountNumber]
      ,[Name]
      ,[IsActive]
      ,[UpdateHash]
	FROM [dbo].[vSubAccounts_sync]
	WHERE [Id] NOT IN (SELECT [Id] FROM [dbo].[vSubAccounts])

	UPDATE [dbo].[vSubAccounts]
	SET [AccountNumber] = t2.[AccountNumber]
		,[SubAccountNumber] = t2.[SubAccountNumber]
		,[Name] = t2.[Name]
		,[IsActive] = t2.[IsActive]
		,[UpdateHash] = t2.[UpdateHash]
	FROM [dbo].[vSubAccounts] t1
	INNER JOIN [dbo].[vSubAccounts_sync]  t2 ON t1.Id = t2.Id

	--TRUNCATE TABLE [dbo].[vSubAccounts_sync]
END