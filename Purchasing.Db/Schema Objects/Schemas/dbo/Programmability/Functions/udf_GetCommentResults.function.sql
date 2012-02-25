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
-- DECLARE @ContainsSearchCondition varchar(255) = 'handle care' 
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetCommentResults(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- OrderId	RequestNumber	DateCreated					Text											CreatedBy
-- 4		ACRU-DGAJOAS	2012-02-23 09:35:23.0000000	Please handle with care, these books are old	Scott Kirkland
--
-- Modifications:
--	2012-02-24 by kjt: Replaced CONTAINS with FREETEXT as per Scott Kirkland.
-- =============================================
CREATE FUNCTION udf_GetCommentResults 
(	
	-- Add the parameters for the function here
	@UserId varchar(10), 
	@ContainsSearchCondition varchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
  SELECT TOP 100 PERCENT OC.[OrderId]
      ,[RequestNumber]
      ,OC.[DateCreated]
      ,[Text]
      ,[FirstName] + ' ' + [LastName] AS [CreatedBy]
  FROM [PrePurchasing].[dbo].[OrderComments] OC
  INNER JOIN [PrePurchasing].[dbo].[Orders]	 O ON OC.[OrderId] = O.[Id]
  INNER JOIN [PrePurchasing].[dbo].[Users] U ON OC.[UserID] = U.[Id]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON OC.[OrderId] = A.[OrderId] 
  WHERE FREETEXT([text], @ContainsSearchCondition) AND A.[AccessUserId] = @UserId AND A.[isadmin] = 0 )