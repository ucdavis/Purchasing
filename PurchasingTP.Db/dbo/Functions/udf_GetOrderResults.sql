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
--	2012-02-27 by kjt: Added table name alias as per Alan Lai; Revised to use alternate syntax that defines table variable first.
--	2012-03-02 by kjt: Revised to include filter rank as per Scott Kirkland.
-- =============================================
CREATE FUNCTION [dbo].[udf_GetOrderResults]
(	
	-- Add the parameters for the function here
	@UserId varchar(10), --User ID of currently logged in user.
	@ContainsSearchCondition varchar(255) --A string containing the word or words to search on.
)
RETURNS @returntable TABLE 
(
	Id int not null
	,DateCreated datetime not null
	,DeliverTo varchar(50) not null
	,DeliverToEmail varchar(50) null
	,Justification varchar(max) not null
	,CreatedBy varchar(101) not null
	,RequestNumber varchar(20) not null
)
AS
BEGIN 
  INSERT INTO @returntable
  SELECT O.[Id]
      ,O.[DateCreated]
      ,O.[DeliverTo]
      ,O.[DeliverToEmail]
      ,O.[Justification]
      ,U.[FirstName] + ' ' + U.[LastName] AS [CreatedBy]
      ,O.[RequestNumber]
  FROM [PrePurchasing].[dbo].[Orders] O
  INNER JOIN [PrePurchasing].[dbo].[Users] U ON O.[CreatedBy] = U.[Id]
  INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON O.[Id] = A.[OrderId] 
  INNER JOIN FREETEXTTABLE([Orders], ([Justification], [RequestNumber], [DeliverTo], [DeliverToEmail]), @ContainsSearchCondition) KEY_TBL on O.Id = KEY_TBL.[KEY]
  WHERE A.[AccessUserId] = @UserId AND A.[isadmin] = 0 

  ORDER BY KEY_TBL.[RANK] DESC

RETURN
END
