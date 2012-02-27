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
-- DECLARE @ContainsSearchCondition varchar(255) = 'reading fun Smith Lai ACRU'
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetOrderResults(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- Id	DateCreated				DeliverTo		DeliverToEmail		Justification					CreatedBy		RequestNumber
-- 2	2012-02-22 20:53:35.000	Alan Lai		anlai@ucdavis.edu	Justification					Alan Lai		ACRU-EFTT2H9
-- 3	2012-02-23 07:58:25.000	Frank Grimes	NULL				Justification					Frank Grimes	ACRU-FJEZEK6
-- 4	2012-02-23 09:35:22.000	Mr. Smith		msmith@ucdavis.edu	Because reading is fun-damental	Scott Kirkland	ACRU-DGAJOAS
-- 5	2012-02-23 10:33:41.000	Mr. Smith		msmith@ucdavis.edu	Because reading is fun-damental	Scott Kirkland	ACRU-FJ6GZIL
-- 6	2012-02-23 10:39:19.000	Mr. Smith		msmith@ucdavis.edu	Because reading is fun-damental	Scott Kirkland	ACRU-DCZDRMJ
-- 7	2012-02-23 11:21:04.000	Mr. Smith		msmith@ucdavis.edu	Because reading is fun-damental	Scott Kirkland	ACRU-C1L5RCV
--
-- Modifications:
--	2012-02-24 by kjt: Replaced CONTAINS with FREETEXT as per Scott Kirkland.
--	2012-02-27 by kjt: Revised to use [PrePurchasing].[dbo].[OrderResults] view
-- =============================================
CREATE FUNCTION [dbo].[udf_GetOrderResults]
(	
	-- Add the parameters for the function here
	@UserId varchar(10), 
	@ContainsSearchCondition varchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
  SELECT [PrePurchasing].[dbo].[vOrderResults].[Id]
      ,[PrePurchasing].[dbo].[vOrderResults].[DateCreated]
      ,[PrePurchasing].[dbo].[vOrderResults].[DeliverTo]
      ,[PrePurchasing].[dbo].[vOrderResults].[DeliverToEmail]
      ,[PrePurchasing].[dbo].[vOrderResults].[Justification]
      ,[PrePurchasing].[dbo].[vOrderResults].[CreatedBy]
      ,[PrePurchasing].[dbo].[vOrderResults].[RequestNumber]
  FROM [PrePurchasing].[dbo].[vOrderResults]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] ON [PrePurchasing].[dbo].[vOrderResults].[Id] = [PrePurchasing].[dbo].[vAccess].[OrderId] 
  WHERE FREETEXT(([PrePurchasing].[dbo].[vOrderResults].[Justification], [PrePurchasing].[dbo].[vOrderResults].[RequestNumber], [PrePurchasing].[dbo].[vOrderResults].[DeliverTo], [PrePurchasing].[dbo].[vOrderResults].[DeliverToEmail]), @ContainsSearchCondition) AND [PrePurchasing].[dbo].[vAccess].[AccessUserId] = @UserId AND [PrePurchasing].[dbo].[vAccess].[isadmin] = 0 
)*/