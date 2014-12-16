CREATE TABLE [dbo].[vVendorAddresses_copy] (
    [Id]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [VendorId]    CHAR (10)        NOT NULL,
    [TypeCode]    VARCHAR (4)      NOT NULL,
    [Name]        VARCHAR (40)     NOT NULL,
    [Line1]       VARCHAR (45)     NULL,
    [Line2]       VARCHAR (45)     NULL,
    [Line3]       VARCHAR (45)     NULL,
    [City]        VARCHAR (45)     NULL,
    [State]       CHAR (2)         NULL,
    [Zip]         VARCHAR (20)     NULL,
    [CountryCode] VARCHAR (2)      NULL,
    [PhoneNumber] VARCHAR (40)     NULL,
    [FaxNumber]   VARCHAR (40)     NULL,
    [Email]       VARCHAR (90)     NULL,
    [Url]         VARCHAR (200)    NULL,
    [IsDefault]   BIT              NULL,
    [IsActive]    BIT              NULL,
    [UpdateHash]  VARBINARY (16)   NULL,
    CONSTRAINT [PK_vVendorAddresses_copy] PRIMARY KEY CLUSTERED ([VendorId] ASC, [TypeCode] ASC)
);

