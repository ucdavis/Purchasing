CREATE TABLE [dbo].[vVendorAddresses] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [VendorId]        CHAR (10)        NOT NULL,
    [TypeCode]        VARCHAR (4)      NOT NULL,
    [Name]            VARCHAR (40)     NOT NULL,
    [Line1]           VARCHAR (40)     NOT NULL,
    [Line2]           VARCHAR (40)     NULL,
    [Line3]           VARCHAR (40)     NULL,
    [City]            VARCHAR (40)     NOT NULL,
    [State]           CHAR (2)         NULL,
    [Zip]             VARCHAR (11)     NULL,
    [CountryCode]     VARCHAR (2)      NULL,
	[PhoneNumber]	  VARCHAR (15)	   NULL,
	[FaxNumber]		 VARCHAR(15) NULL,
	[Email] VARCHAR(50) NULL, 
    [Url] VARCHAR(128) NULL,
    [PartitionColumn] INT              NOT NULL
    
) ON [EvenOddPartitionScheme] ([PartitionColumn]);







