USE [PrePurchasing]
GO
/****** Object:  UserDefinedFunction [dbo].[udf_GetLineResults]    Script Date: 5/7/2012 4:03:14 PM ******/
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
--	2012-02-27 by kjt: Added table alias as per Alan Lai; Revised to use alternate syntax that defines table variable first.
--	2012-03-02 by kjt: Revised to include filter rank as per Scott Kirkland.
-- =============================================
ALTER FUNCTION [dbo].[udf_GetLineResults] 
(	
	-- Add the parameters for the function here
	@UserId varchar(10), --User ID of currently logged in user.
	@ContainsSearchCondition varchar(255) --A string containing the word or words to search on.
)
RETURNS @returntable TABLE 
(
	OrderId int not null
	,Quantity decimal(18,3) not null
	,Unit varchar(25) null
	,RequestNumber varchar(20) not null
	,CatalogNumber varchar(25) null
	,[Description] varchar(max) not null
	,Url varchar(200) null
	,Notes varchar(max) null
	,CommodityId varchar(9) null
	,ReceivedNotes varchar(max) null
)
AS
BEGIN
	INSERT INTO @returntable
	SELECT TOP 100 PERCENT LI.[OrderId]
      ,LI.[Quantity]
      ,LI.[Unit]
      ,O.[RequestNumber]
      ,LI.[CatalogNumber]
      ,LI.[Description]
      ,LI.[Url]
      ,LI.[Notes]
      ,LI.[CommodityId]
	  ,LI.[ReceivedNotes]
  FROM [PrePurchasing].[dbo].[LineItems] LI
  INNER JOIN [PrePurchasing].[dbo].[Orders]	 O ON LI.[OrderId] = O.[Id]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON LI.[OrderId] = A.[OrderId] 
  INNER JOIN FREETEXTTABLE([LineItems], ([Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes]), @ContainsSearchCondition) KEY_TBL on LI.Id = KEY_TBL.[KEY]
  WHERE A.[AccessUserId] = @UserId AND A.[isadmin] = 0 
  ORDER BY KEY_TBL.[RANK] DESC

RETURN
END
