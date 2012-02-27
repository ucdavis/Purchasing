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
-- DECLARE @ContainsSearchCondition varchar(255) = 'space invades' 
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetLineResults(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- OrderId	Quantity	Unit	RequestNumber	CatalogNumber	Description									Url		Notes										CommodityId
-- 4		5.000		EA		ACRU-DGAJOAS	VSD23			First Print Mark Twain Copies of Tom Sawyer	NULL	The space he invades he gets high on you	75080
-- 5		5.000		EA		ACRU-FJ6GZIL	VSD23			First Print Mark Twain Copies of Tom Sawyer	NULL	The space he invades he gets high on you	75080
-- 6		5.000		EA		ACRU-DCZDRMJ	VSD23			First Print Mark Twain Copies of Tom Sawyer	NULL	The space he invades he gets high on you	75080
--
-- Modifications:
--	2012-02-24 by kjt: Replaced CONTAINS with FREETEXT as per Scott Kirkland.
--	2012-02-27 by kjt: Revised to use vLineResults.
-- =============================================
CREATE FUNCTION [dbo].[udf_GetLineResults] 
(	
	-- Add the parameters for the function here
	@UserId varchar(10), 
	@ContainsSearchCondition varchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [PrePurchasing].[dbo].[vLineResults].[OrderId]
      ,[PrePurchasing].[dbo].[vLineResults].[Quantity]
      ,[PrePurchasing].[dbo].[vLineResults].[Unit]
      ,[PrePurchasing].[dbo].[vLineResults].[RequestNumber]
      ,[PrePurchasing].[dbo].[vLineResults].[CatalogNumber]
      ,[PrePurchasing].[dbo].[vLineResults].[Description]
      ,[PrePurchasing].[dbo].[vLineResults].[Url]
      ,[PrePurchasing].[dbo].[vLineResults].[Notes]
      ,[PrePurchasing].[dbo].[vLineResults].[CommodityId]
  FROM [PrePurchasing].[dbo].[vLineResults] 
  INNER JOIN [PrePurchasing].[dbo].[vAccess] ON [PrePurchasing].[dbo].[vLineResults].[OrderId] = [PrePurchasing].[dbo].[vAccess].[OrderId] 
  WHERE FREETEXT(([PrePurchasing].[dbo].[vLineResults].[Description], [PrePurchasing].[dbo].[vLineResults].[Url], [PrePurchasing].[dbo].[vLineResults].[Notes], [PrePurchasing].[dbo].[vLineResults].[CatalogNumber], [PrePurchasing].[dbo].[vLineResults].[CommodityId]), @ContainsSearchCondition) AND [PrePurchasing].[dbo].[vAccess].[AccessUserId] = @UserId AND [PrePurchasing].[dbo].[vAccess].[isadmin] = 0 
)*/