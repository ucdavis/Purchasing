
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 8, 2012
-- Description:	Create the VendorAddresses Partition Tables
-- =============================================
CREATE PROCEDURE usp_CreateVendorAddressesPartitionTable
	-- Add the parameters for the stored procedure here
	@TableNamePrefix varchar(255) = 'vVendorAddressesPartitionTable', --Default table name prefix
	@Mode smallint = 1, --0 to create all three tables; 1 to create main table only (default); 2 to create load tables only.
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
	DECLARE @TableNamePrefix varchar(255) = 'vVendorAddressesPartitionTable'
	DECLARE @IsDebug bit = 0
*/
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;  --Keeps from displaying (3 row(s) affected) message after inserting table names into @TableNameTable.
	--SET NOCOUNT OFF
	
	DECLARE @TableNameSuffixTable TABLE (TableName varchar(255))
	IF @Mode = 1
		INSERT INTO @TableNameSuffixTable VALUES ('')
	ELSE IF @Mode = 2
		INSERT INTO @TableNameSuffixTable VALUES ('_Load'), ('_Empty')
	ELSE
		INSERT INTO @TableNameSuffixTable VALUES (''), ('_Load'), ('_Empty')
		
	DECLARE @TSQL varchar(MAX) = ''
	DECLARE @TableNameSuffix varchar(10) = '' 

	DECLARE myCursor CURSOR FOR SELECT TableName FROM @TableNameSuffixTable

	OPEN myCursor

	FETCH NEXT FROM myCursor INTO @TableNameSuffix

	WHILE @@FETCH_STATUS <> -1
	BEGIN 
		SELECT @TSQL = '
			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @TableNamePrefix + @TableNameSuffix + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '](
				[Id] [uniqueidentifier] NOT NULL,
				[VendorId] [char](10) NOT NULL,
				[TypeCode] [varchar](4) NOT NULL,
				[Name] [varchar](40)  NOT NULL,
				[Line1] [varchar](40) NOT NULL,
				[Line2] [varchar](40) NULL,
				[Line3] [varchar](40) NULL,
				[City] [varchar](40)  NOT NULL,
				[State] [char](2) NULL,
				[Zip] [varchar](11)  NULL,
				[CountryCode] [varchar](2) NULL,
				[PartitionColumn] [int] NOT NULL,
				CONSTRAINT [PK_' + @TableNamePrefix + @TableNameSuffix + '] PRIMARY KEY CLUSTERED
			(
				[VendorId] ASC,
				[TypeCode] ASC,
				[PartitionColumn] ASC
			) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF))
			ON EvenOddPartitionScheme (PartitionColumn)

			SET ANSI_PADDING ON

			ALTER TABLE [dbo].[' + @TableNamePrefix + @TableNameSuffix + '] ADD  CONSTRAINT [DF_' + @TableNamePrefix + @TableNameSuffix + '_Id]  DEFAULT (newid()) FOR [Id]
		'
		
		IF @IsDebug = 1 PRINT @TSQL ELSE EXEC(@TSQL)
		FETCH NEXT FROM myCursor INTO @TableNameSuffix
	END

	CLOSE myCursor
	DEALLOCATE myCursor
END