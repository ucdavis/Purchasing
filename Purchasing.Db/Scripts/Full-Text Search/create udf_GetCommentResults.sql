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
--	2012-02-27 by kjt: Added table alias as per Alan Lai; Revised to use alternate syntax that defines table variable first.
--	2012-03-02 by kjt: Revised to include filter rank as per Scott Kirkland.
-- =============================================
ALTER FUNCTION udf_GetCommentResults 
(	
    -- Add the parameters for the function here
    @UserId varchar(10), --User ID of currently logged in user.
    @ContainsSearchCondition varchar(255) --A string containing the word or words to search on.
)
RETURNS @returntable TABLE 
(
    OrderId int not null
    ,RequestNumber varchar(20) not null
    ,DateCreated datetime2(7) not null
    ,[Text] varchar(max) not null
    ,CreatedBy varchar(101) not null
)
AS
BEGIN
  INSERT INTO @returntable
  SELECT OC.[OrderId]
      ,O.[RequestNumber]
      ,OC.[DateCreated]
      ,OC.[Text]
      ,U.[FirstName] + ' ' + U.[LastName] AS [CreatedBy]
  FROM [PrePurchasing].[dbo].[OrderComments] OC
  INNER JOIN [PrePurchasing].[dbo].[Orders]	 O ON OC.[OrderId] = O.[Id]
  INNER JOIN [PrePurchasing].[dbo].[Users] U ON OC.[UserID] = U.[Id]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON OC.[OrderId] = A.[OrderId] 
  INNER JOIN FREETEXTTABLE([OrderComments], [text], @ContainsSearchCondition) KEY_TBL on OC.Id = KEY_TBL.[KEY]
  WHERE A.[AccessUserId] = @UserId AND A.[isadmin] = 0
  ORDER BY KEY_TBL.[RANK] DESC

  RETURN 
END
