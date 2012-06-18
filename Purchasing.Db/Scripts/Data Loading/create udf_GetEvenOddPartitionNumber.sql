USE [PrePurchasing]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 7, 2012
-- Description:	Get the partition number for the date provided
-- Usage: 
-- DECLARE @MyDate datetime = convert(datetime, '02-07-2012')
-- Or -- SELECT @MyDate = GetDate()
-- select dbo.udf_GetEvenOddPartitionNumber(@MyDate)
-- =============================================
CREATE FUNCTION udf_GetEvenOddPartitionNumber 
(
	-- Add the parameters for the function here
	@InputDate datetime
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result int

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = (select dbo.udf_GetJulianDate(@InputDate)%2 + 1)

	-- Return the result of the function
	RETURN @Result

END
GO

