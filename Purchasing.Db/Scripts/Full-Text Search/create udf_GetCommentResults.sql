USE [PrePurchasing]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
-- DECLARE @ContainsSearchCondition varchar(255) = 'handle AND care' 
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetCommentResults(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- Id	Text											DateCreated					UserId	OrderId
-- 13	Please	handle with care, these books are old	2012-02-23 09:35:23.0000000	postit	4
--
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
      ,[CreatedBy]
  FROM [PrePurchasing].[dbo].[OrderComments] OC
  INNER JOIN [PrePurchasing].[dbo].[Orders]	 O ON OC.[OrderId] = O.[Id]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON OC.[OrderId] = A.[OrderId] 
  WHERE CONTAINS([text], @ContainsSearchCondition) AND A.[AccessUserId] = @UserId AND A.[isadmin] = 0 
)
GO
