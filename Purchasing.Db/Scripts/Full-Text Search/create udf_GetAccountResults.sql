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
 
--DECLARE @ContainsSearchCondition varchar(255) = '3-216 way' 
 
--SELECT * from udf_GetAccountResults(@ContainsSearchCondition)
-- 
-- results:
-- Id			Name
-- 3-SCWAWWS	WATER WAYS:WATER ED PROG FOR URBAN YOUTH
-- 3-VEN0216	VE:  VEN 216
-- L-26932UB	USDA FAS 58-3148-8-216
-- L-9226816	USDA-NRCS 74-9104-9-216
-- L-9226932	USDA FAS 58-3148-8-216
-- L-AIVU216	CA&ES:IVO:USDA58-3148-8-216-1 MONTENEGRO
-- S-IDPR829	MED: IMID: Gilead GS-US-216-0114/Pollard
-- 3-12LARA1	LARA DOWNS - 13 WAYS - 11/12 - 11/13/11
-- 3-32368UB	UCSD MCA R/CONT-216
-- 3-84629UB	Gilead Sciences award #GS-US-216-0114
-- 3-9232368	UCSD MCA R/CONT-216
-- 3-9257238	AMER HEART ASSN #96-216
-- 3-9280128	USDA 58-5348-8-216
-- 3-9284629	Gilead Sciences award #GS-US-216-0114
--
-- Modifications:
-- =============================================
ALTER FUNCTION [dbo].[udf_GetAccountResults]
(
	@ContainsSearchCondition varchar(255)
)
RETURNS @returntable TABLE 
(
	 Id varchar(10) not null
	,Name varchar(50) not null
)
AS
BEGIN
	INSERT INTO @returntable
	SELECT [Id]
		  ,[Name]
	FROM [PrePurchasing].[dbo].[vAccounts]
	WHERE FREETEXT([Name], @ContainsSearchCondition)
RETURN
END