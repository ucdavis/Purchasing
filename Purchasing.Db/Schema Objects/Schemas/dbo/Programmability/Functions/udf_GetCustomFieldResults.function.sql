/*-- =============================================
-- Author:		Ken Taylor
-- Create date: February 23, 2012
-- Description:	Given an UserId (Kerberos) and ContainsSearchCondition search string, 
-- return the non-admin records matching the search string that the user can see
--
-- Usage:
-- USE [PrePurchasing]
-- GO
-- 
-- DECLARE @ContainsSearchCondition varchar(255) = 'custom answer' 
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetCustomFieldResults(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- OrderId	RequestNumber	Question	Answer
-- 7		ACRU-C1L5RCV	RUA #		Custom answer
--
-- Modifications:
--	2012-02-24 by kjt: Replaced CONTAINS with FREETEXT as per Scott Kirkland.
--	2012-02-27 by kjt: Added table alias as per Alan Lai; Revised to use vCustomFieldResults view.
-- =============================================
CREATE FUNCTION [dbo].[udf_GetCustomFieldResults]
(	
	-- Add the parameters for the function here
	@UserId varchar(10), 
	@ContainsSearchCondition varchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
SELECT [PrePurchasing].[dbo].[vCustomFieldResults].[OrderId]
      ,[PrePurchasing].[dbo].[vCustomFieldResults].[RequestNumber]
      ,[PrePurchasing].[dbo].[vCustomFieldResults].[Question]
      ,[PrePurchasing].[dbo].[vCustomFieldResults].[Answer]
  FROM [PrePurchasing].[dbo].[vCustomFieldResults]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] ON [PrePurchasing].[dbo].[vCustomFieldResults].[OrderId] = [PrePurchasing].[dbo].[vAccess].[OrderId] 
  WHERE FREETEXT([PrePurchasing].[dbo].[vCustomFieldResults].[Answer], @ContainsSearchCondition) AND [PrePurchasing].[dbo].[vAccess].[AccessUserId] = @UserId AND [PrePurchasing].[dbo].[vAccess].[isadmin] = 0 
)*/