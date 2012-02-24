-- =============================================
-- Author:		Ken Taylor
-- Create date: February 23, 2012
-- Description:	Given an UserId (Kerberos) and ContainsSearchCondition search string, 
-- return the non-admin records matching the search string that the user can see
--
-- Usage:
-- USE [PrePurchasing]
-- GO
-- 
-- DECLARE @ContainsSearchCondition varchar(255) = 'custom AND answer' 
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetCustomFieldResults(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- OrderId	RequestNumber	Question	Answer
-- 7		ACRU-C1L5RCV	RUA #		Custom answer
--
-- =============================================
CREATE FUNCTION udf_GetCustomFieldResults 
(	
	-- Add the parameters for the function here
	@UserId varchar(10), 
	@ContainsSearchCondition varchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
SELECT TOP 100 PERCENT CFA.[OrderId]
      ,[RequestNumber]
      ,[Name] AS [Question]
      ,[Answer]
  FROM [PrePurchasing].[dbo].[CustomFieldAnswers] CFA
  INNER JOIN [PrePurchasing].[dbo].[CustomFields] CF ON CFA.[CustomFieldId] = CF.[Id]
  INNER JOIN [PrePurchasing].[dbo].[Orders]	 O ON CFA.[OrderId] = O.[Id]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON CFA.[OrderId] = A.[OrderId] 
  WHERE CONTAINS([Answer], @ContainsSearchCondition) AND A.[AccessUserId] = @UserId AND A.[isadmin] = 0 
)