-- =============================================
-- Author:		Ken Taylor
-- Create date: February 8, 2012
-- Description:	Create the Vendors table.
-- Modifications: 
--	2012-03-21 by kjt: Added IsActive bit as per Alan Lai.
-- Modifications:
--	2012-06-23 by kjt: Converted from partitioned/swap table loading to direct table loading.
-- =============================================
CREATE PROCEDURE [dbo].[usp_CreateVendorsTable]
	-- Add the parameters for the stored procedure here
	@LoadTableName varchar(255) = 'vVendorsTable', --Default table name prefix
	@IsDebug bit = 0 --Set to 1 just print the SQL and not actually execute it. 
AS
BEGIN
/*
DECLARE @LoadTableName varchar(255) = 'vVendors'
DECLARE @IsDebug bit = 0
*/

	SET NOCOUNT ON --Keeps from displaying (3 row(s) affected) message after inserting table names into @TableNameTable.
	--SET NOCOUNT OFF
	
	DECLARE @TSQL varchar(MAX) = ''

		SELECT @TSQL = '
			IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[' + @LoadTableName + ']'') AND type in (N''U''))
			BEGIN
				DROP TABLE [dbo].[' + @LoadTableName + ']
			END

			SET ANSI_NULLS ON

			SET QUOTED_IDENTIFIER ON

			SET ANSI_PADDING ON

			CREATE TABLE [dbo].[' + @LoadTableName + '](
				[Id] [varchar](10) NOT NULL,
				[Name] [varchar](40) NOT NULL,
				[OwnershipCode] [varchar](2) NULL,
				[BusinessTypeCode] [varchar](2) NULL,
				[IsActive] [bit] NULL,
				
				CONSTRAINT [PK_' + @LoadTableName + '] PRIMARY KEY CLUSTERED
			(
				[Id] ASC
			) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = OFF, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
) ON [PRIMARY]

			SET ANSI_PADDING ON
	'

		IF @IsDebug = 1 
			PRINT @TSQL 
		ELSE 
			EXEC(@TSQL)
END
GO

