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
--	2012-02-27 by kjt: Added table alias as per Alan Lai; Revised to use vCommentResults view.
-- =============================================
CREATE FUNCTION [dbo].[udf_GetCommentResults] 
(	
	-- Add the parameters for the function here
	@UserId varchar(10), 
	@ContainsSearchCondition varchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
  SELECT [PrePurchasing].[dbo].[vCommentResults].[OrderId]
      ,[PrePurchasing].[dbo].[vCommentResults].[RequestNumber]
      ,[PrePurchasing].[dbo].[vCommentResults].[DateCreated]
      ,[PrePurchasing].[dbo].[vCommentResults].[Text]
      ,[PrePurchasing].[dbo].[vCommentResults].[CreatedBy]
  FROM [PrePurchasing].[dbo].[vCommentResults]
  INNER JOIN [PrePurchasing].[dbo].[vAccess]  ON [PrePurchasing].[dbo].[vCommentResults].[OrderId] = [PrePurchasing].[dbo].[vAccess].[OrderId] 
  WHERE FREETEXT([PrePurchasing].[dbo].[vCommentResults].[text], @ContainsSearchCondition) AND [PrePurchasing].[dbo].[vAccess].[AccessUserId] = @UserId AND [PrePurchasing].[dbo].[vAccess].[isadmin] = 0 )*/