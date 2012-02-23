-- =============================================
-- Author:		Ken Taylor
-- Create date: February 23, 2012
-- Description:	Given an UserId (kerberos) and ContainsSearchCondition search string, 
-- return the non-admin records matching the search string that the user can see
--
-- Usage:
-- USE [PrePurchasing]
-- GO
-- 
-- DECLARE @ContainsSearchCondition varchar(255) = 'reading AND fun' 
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetOrderResults(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- Id	DateCreated				DeliverTo	DeliverToEmail		Justification					CreatedBy	RequestNumber
-- 4	2012-02-23 09:35:22.000	Mr. Smith	msmith@ucdavis.edu	Because reading is fun-damental	postit		ACRU-DGAJOAS
--
-- =============================================
CREATE FUNCTION udf_GetOrderResults
(	
	-- Add the parameters for the function here
	@UserId varchar(10), 
	@ContainsSearchCondition varchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT TOP 100 PERCENT O.[Id]
      ,[DateCreated]
      ,[DeliverTo]
      ,[DeliverToEmail]
      ,[Justification]
      ,[CreatedBy]
      ,[RequestNumber]
  FROM [PrePurchasing].[dbo].[Orders] O
  INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON O.Id = A.OrderId 
  WHERE CONTAINS(justification, @ContainsSearchCondition) AND A.AccessUserId = @UserId AND A.isadmin = 0 
)