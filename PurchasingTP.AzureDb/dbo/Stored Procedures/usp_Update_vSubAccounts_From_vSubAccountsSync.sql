
-- Modifications:
-- 2018-06-01 by kjt: Revised to use NOT exists as opposed to using the ID, since this is a GUID that is
-- only valid if we're trying to sync the data from the same source and previously used; otherwise, the 
-- id (GUID) will most likely be different since it was generated on a different host.
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
	FROM [dbo].[vSubAccounts_sync] t1
	WHERE  NOT EXISTS
   (	SELECT [AccountNumber], [SubAccountNumber] 
		FROM [dbo].[vSubAccounts] t2 
		WHERE t2.[AccountNumber]    = t1.[AccountNumber] AND 
			  t2.[SubAccountNumber] = t1.[SubAccountNumber] )

	UPDATE [dbo].[vSubAccounts]
	SET  [id] = t2. [id]
		,[AccountNumber] = t2.[AccountNumber]
		,[SubAccountNumber] = t2.[SubAccountNumber]
		,[Name] = t2.[Name]
		,[IsActive] = t2.[IsActive]
		,[UpdateHash] = t2.[UpdateHash]
	FROM [dbo].[vSubAccounts] t1
	INNER JOIN [dbo].[vSubAccounts_sync]  t2 ON  
		t2.[AccountNumber]    = t1.[AccountNumber] AND 
		t2.[SubAccountNumber] = t1.[SubAccountNumber]

	--TRUNCATE TABLE [dbo].[vSubAccounts_sync]
END