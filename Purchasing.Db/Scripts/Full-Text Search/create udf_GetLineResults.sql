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
-- DECLARE @ContainsSearchCondition varchar(255) = 'space AND invades' 
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetLineResult(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- OrderId	Quantity	Unit	RequestNumber	CatalogNumber	Description									Url		Notes										CommodityId
-- 4		5.000		EA		ACRU-DGAJOAS	VSD23			First Print Mark Twain Copies of Tom Sawyer	NULL	The space he invades he gets high on you	75080
-- 5		5.000		EA		ACRU-FJ6GZIL	VSD23			First Print Mark Twain Copies of Tom Sawyer	NULL	The space he invades he gets high on you	75080
-- 6		5.000		EA		ACRU-DCZDRMJ	VSD23			First Print Mark Twain Copies of Tom Sawyer	NULL	The space he invades he gets high on you	75080
--
-- =============================================
ALTER FUNCTION udf_GetLineResults 
(	
	-- Add the parameters for the function here
	@UserId varchar(10), 
	@ContainsSearchCondition varchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT TOP 100 PERCENT LI.[OrderId]
      ,[Quantity]
      ,[Unit]
      ,[RequestNumber]
      ,[CatalogNumber]
      ,[Description]
      ,[Url]
      ,[Notes]
      ,[CommodityId]
  FROM [PrePurchasing].[dbo].[LineItems] LI
  INNER JOIN [PrePurchasing].[dbo].[Orders]	 O ON LI.[OrderId] = O.[Id]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON LI.[OrderId] = A.[OrderId] 
  WHERE CONTAINS(([Description], [Url], [Notes], [CatalogNumber], [CommodityId]), @ContainsSearchCondition) AND A.[AccessUserId] = @UserId AND A.[isadmin] = 0 
)
GO
