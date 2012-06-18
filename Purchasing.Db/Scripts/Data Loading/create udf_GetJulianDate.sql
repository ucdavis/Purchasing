USE
[PrePurchasing]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 6, 2010
-- Description:	Returns the Julian date
-- Usage:
-- select dbo.udf_GetJulian(NULL) --for today's Julian date or
--
-- DECLARE @MyDate datetime = convert(datetime, '12-31-2011')
-- select dbo.udf_GetJulian(@MyDate) --for the Julian date of the date provided.
-- =============================================
CREATE FUNCTION udf_GetJulianDate 
(
	@Today datetime = NULL
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result int
	
	IF @Today IS NULL SELECT @Today = GetDate()

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = (select datediff(d, convert(datetime,('01/01/'+convert(char(4),year(@Today)))),@Today)+1)

	-- Return the result of the function
	RETURN @Result

END
GO

