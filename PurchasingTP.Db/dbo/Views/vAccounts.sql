﻿
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
     
  FROM [PrePurchasingLookups].[dbo].[vAccounts]