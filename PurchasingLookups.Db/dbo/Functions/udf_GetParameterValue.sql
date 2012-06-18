-- =============================================
-- Author:		Ken Taylor
-- Create date: January 12, 2012
-- Description:	Given an input parameter and a default, return the appropriate value
-- Note: Value can be disabled to force return of dafault value by setting row's IsActive bit to false.
--
-- Precedence:
-- 1. Return the @InputParamValue unless null or blank, i.e. ''
-- 2. Else return the table value unless null, blank or IsActive = false.
-- 3. Otherwise return the @DefaultValue (provided).
--
-- Usage:
-- DECLARE @AdminTableNameDefault varchar(255) = 'AdminTable'
-- DECLARE @InputParamValue varchar(255) = '' -- The value passed in by the input parameter (if present)
-- DECLARE @DefaultValue varchar(255) = (SELECT @AdminTableNameDefault) -- The default value for the parameter if not passed in or present in the [AD419].[dbo].[ParamNameAndValue] table
-- DECLARE @ParamVariableName varchar(255) = 'AdminTableName' -- The name of the variable if present in the [AD419].[dbo].[ParamNameAndValue] table
--
-- SELECT  dbo.udf_GetParameterValue (@InputParamValue, @DefaultValue, @ParamVariableName )
-- SELECT dbo.udf_GetParameterValue ('MyPassedInParamValue', 'MyDefaultParamValue', 'MyVariableName' )
-- eg. SELECT dbo.udf_GetParameterValue ('TestAdminTable', 'AdminTable', 'AdminTableName')
-- eg. SELECT dbo.udf_GetParameterValue ('TestAdminTable', 'AdminTable', 'AdminTableName' )
-- =============================================
CREATE FUNCTION [dbo].[udf_GetParameterValue] 
(
	-- Add the parameters for the function here
	@InputParamValue varchar(255), --The value of the input parameter
	@DefaultValue varchar(255), --The default value for the parameter
	@ParamVariableName varchar(255) --The variable name of the parameter as saved in TableNameVariableName column of [AD419].[dbo].[ParameterNameAndValue]
)
RETURNS varchar(255)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @OutputParamValue varchar(255) = @InputParamValue

	-- Add the T-SQL statements to compute the return value here
	-- 1. use param if provided, else use database, else use default 
	IF @InputParamValue IS NULL OR @InputParamValue LIKE ''
	-- No param provided
		BEGIN	
		-- if table exists and value is not null of '' use table name's value
			IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ParamNameAndValue]') AND type in (N'U'))
				BEGIN
					SELECT @OutputParamValue = (SELECT ParamValue FROM [PrePurchasingLookups].[dbo].[ParamNameAndValue] WHERE [ParamName] = @ParamVariableName AND [IsActive] = 1)
					IF (@OutputParamValue IS NULL OR @OutputParamValue LIKE '') 
						BEGIN
						-- use default:
							SELECT @OutputParamValue = @DefaultValue
						END
				-- ELSE use table name's value
				END
			ELSE
			-- use default:
				SELECT @OutputParamValue = @DefaultValue
		END
		
	-- Return the result of the function
	RETURN @OutputParamValue

END
