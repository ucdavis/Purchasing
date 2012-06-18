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
-- DECLARE @ContainsSearchCondition varchar(255) = 'custom answer' 
-- DECLARE @UserId varchar(255) = 'anlai' --'jsylvest'
-- 
-- SELECT * from udf_GetCustomFieldResults(@UserId, @ContainsSearchCondition)
-- 
-- results:
-- OrderId	RequestNumber	Question	Answer
-- 7		ACRU-C1L5RCV	RUA #		Custom answer
--
-- Modifications:
--	2012-02-24 by kjt: Replaced CONTAINS with FREETEXT as per Scott Kirkland.
--	2012-02-27 by kjt: Added table alias as per Alan Lai; Revised to use alternate syntax that defines table variable first.
--	2012-03-02 by kjt: Revised to include filter rank as per Scott Kirkland.
-- =============================================
ALTER FUNCTION [dbo].[udf_GetCustomFieldResults]
(
    @UserId varchar(10), --User ID of currently logged in user.
    @ContainsSearchCondition varchar(255) --A string containing the word or words to search on.
)
RETURNS @returntable TABLE 
(
     OrderId int not null
    ,RequestNumber varchar(20) not null
    ,Question varchar(max) not null
    ,Answer varchar(max) not null
)
AS
BEGIN
    INSERT INTO @returntable
    SELECT CFA.[OrderId]
      ,O.[RequestNumber]
      ,CF.[Name] AS [Question]
      ,CFA.[Answer]
    FROM [PrePurchasing].[dbo].[CustomFieldAnswers] CFA 
    INNER JOIN [PrePurchasing].[dbo].[CustomFields] CF ON CFA.[CustomFieldId] = CF.[Id]
    INNER JOIN [PrePurchasing].[dbo].[Orders]  O ON CFA.[OrderId] = O.[Id]
    INNER JOIN [PrePurchasing].[dbo].[vAccess] A ON CFA.[OrderId] = A.[OrderId] 
    INNER JOIN FREETEXTTABLE([CustomFieldAnswers], [Answer], @ContainsSearchCondition) KEY_TBL on CFA.Id = KEY_TBL.[KEY]
    WHERE A.[AccessUserId] = @UserId AND A.[isadmin] = 0 
    ORDER BY KEY_TBL.[RANK] DESC
    
    RETURN
END