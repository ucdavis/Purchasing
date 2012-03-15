-- =============================================
-- Author:		Ken Taylor
-- Create date: March 13, 2012
-- Description:	Given aContainsSearchCondition search string, 
--				return the records matching the search string.
--
-- Notes: This is an example of a Multi-statement Table-valued function.   
-- This syntax behaves nicely within VS2010 database projects.  However, it requires 
-- nearly twice the number of SQL statements when compared to a similar Inline Table-valued
-- function designed for the identical purpose, plus explicit declaration of the table
-- variable beforehand.
--
-- Usage:
-- USE [PrePurchasing]
-- GO
-- 
-- DECLARE @ContainsSearchCondition varchar(255) = '3209 test' –-Searches on either BuildingCode or BuildingName.
-- SELECT * from udf_GetBuildingResults(@ContainsSearchCondition)
-- 
-- results:
-- Id		CampusCode	BuildingCode	CampusName		CampusShortName	CampusTypeCode	BuildingName								LastUpdateDate			IsActive
-- 3-8187	3			8187			Davis Campus	Davis Campus	B				CAHFS SAN BERNARDINO DAIRY TESTING FACIL	2012-03-12 22:39:29.000	1
-- 3-3209	3			3209			Davis Campus	Davis Campus	B				GROUNDS TOOL HOUSE							2012-03-12 22:39:29.000	1
--
-- Modifications:
-- =============================================
CREATE FUNCTION [dbo].[udf_GetBuildingResults] 
(	
	@ContainsSearchCondition varchar(255) --A string containing the word or words to search on.
)
RETURNS @returntable TABLE 
(
	 Id varchar(10) not null
	,CampusCode varchar(2) not null
	,BuildingCode varchar(4) not null
	,CampusName varchar(40) null
	,CampusShortName varchar(12) null
	,CampusTypeCode varchar(1) null
	,BuildingName varchar(80) null
	,LastUpdateDate datetime null
	,IsActive bit null
)
AS
BEGIN
	INSERT INTO @returntable
	SELECT TOP 100 PERCENT [Id]
      ,[CampusCode]
      ,[BuildingCode]
      ,[CampusName]
      ,[CampusShortName]
      ,[CampusTypeCode]
      ,[BuildingName]
      ,[LastUpdateDate]
      ,[IsActive]
  FROM [PrePurchasing].[dbo].[vBuildings] SEARCH_TBL
  INNER JOIN FREETEXTTABLE(vBuildings, ([BuildingCode], [BuildingName]), @ContainsSearchCondition) KEY_TBL on SEARCH_TBL.Id = KEY_TBL.[KEY]
  ORDER BY KEY_TBL.[RANK] DESC

RETURN
END