
--DROP VIEW [dbo].[vAccounts]

CREATE VIEW [dbo].[vAccounts] --WITH SCHEMABINDING
 AS 
SELECT [Id]
      ,[Name]
      ,[IsActive]
      ,[AccountManager]
      ,[AccountManagerId]
      ,[PrincipalInvestigator]
      ,[PrincipalInvestigatorId]
      ,[OrganizationId]
      ,[PartitionColumn]
  FROM [PrePurchasingLookups].[dbo].[vAccounts]