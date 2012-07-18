CREATE FUNCTION [dbo].[udf_GetAccountResults]
(
	@ContainsSearchCondition varchar(255) --A string containing the word or words to search on.
)
RETURNS @returntable TABLE 
(
	 Id varchar(10) not null
	,Name varchar(50) not null
)
AS
BEGIN
	INSERT INTO @returntable
	SELECT TOP 20
		   [Id]
		  ,[Name]
	FROM [PrePurchasingLookUps].[dbo].[vAccounts] FT_TBL INNER JOIN
	FREETEXTTABLE([vAccounts], ([Id], [Name]), @ContainsSearchCondition) KEY_TBL on FT_TBL.Id = KEY_TBL.[KEY]
	WHERE [IsActive] = 1
	ORDER BY KEY_TBL.[RANK] DESC

	RETURN
END