USE [PrePurchasing]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ken Taylor
-- Create date: March 01, 2012
-- Description:	Given a ContainsSearchCondition search string, 
-- return the records matching the search string provided.
--
-- Usage:
--USE [PrePurchasing]
--GO
 
--DECLARE @ContainsSearchCondition varchar(255) = 'General'
--
--SELECT * from udf_GetVendorResults(@ContainsSearchCondition)
-- 
-- results:
-- Id			Name
-- 0000000068	GENERAL SIGNAL POWER SYSTEMS INC
-- 0000000262	GENERAL TECHNOLOGIES
-- 0000000295	GENERAL PHYSICS CORPORATION
-- 0000000713	GENERAL PENCIL COMPANY
-- 0000000998	PROGRESSIVE PACIFIC GENERAL & ENG CONT
-- 0000001563	GENERAL GLASS & DOOR SERVICE
-- 0000001746	GENERAL GRINDING INC
-- 0000001749	GENERAL MICRO COMPANY
-- 0000002689	MCKESSON GENERAL MEDICAL
-- 0000004610	DIAMOND GENERAL DEVELOPMENT CORP
-- 0000005077	GENERAL COMPUTER SYSTEMS
-- 0000005083	GENERAL EASTERN INSTRUMENTS
-- 0000005088	GENERAL ELECTRONICS SYSTEMS INC
-- 0000005090	GENERAL HALL INC
-- 0000005093	GENERAL METERS CORPORATION
-- 0000005644	JIM REFVEM GENERAL CONTRACTOR
-- 0000006084	LANGILLS GENERAL MACHINE INC
-- 0000008360	WEST GENERAL ASSOCIATES
-- 0000008775	CHERCO GENERAL CONTRACTOR
-- 0000009624	CALIFORNIA GENERAL TIRE
--
-- Modifications:
--	2012-03-02 by kjt: Revised to include filter by TOP 20, and rank as per Scott Kirkland.
--	2012-03-21 by kjt: Added filtering where IsActive = 1 as per Alan Lai.
-- =============================================
ALTER FUNCTION [dbo].[udf_GetVendorResults]
(
	@ContainsSearchCondition varchar(255) --A string containing the word or words to search on.
)
RETURNS @returntable TABLE 
(
	 Id varchar(10) not null
	,Name varchar(40) not null
)
AS
BEGIN
	INSERT INTO @returntable
	SELECT TOP 20
		   [Id]
		  ,[Name]
	FROM [PrePurchasing].[dbo].[vVendors] FT_TBL INNER JOIN
	FREETEXTTABLE([vVendors], [Name], @ContainsSearchCondition) KEY_TBL on FT_TBL.Id = KEY_TBL.[KEY]
	WHERE [IsActive] = 1
	ORDER BY KEY_TBL.[RANK] DESC
	
	RETURN
END