CREATE TABLE [dbo].[vVendorAddresses] (
    [Id]              UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [VendorId]        CHAR (10)        NOT NULL,
    [TypeCode]        VARCHAR (4)      NOT NULL,
    [DetailId]        VARCHAR (4)      NOT NULL,
    [VendorAddressId] VARCHAR (25)     NOT NULL,
    [Name]            VARCHAR (50)     NULL,
    [Line1]           VARCHAR (45)     NULL,
    [Line2]           VARCHAR (45)     NULL,
    [Line3]           VARCHAR (45)     NULL,
    [City]            VARCHAR (45)     NULL,
    [State]           CHAR (2)         NULL,
    [Zip]             VARCHAR (20)     NULL,
    [CountryCode]     VARCHAR (2)      NULL,
    [PhoneNumber]     VARCHAR (40)     NULL,
    [FaxNumber]       VARCHAR (40)     NULL,
    [Email]           VARCHAR (90)     NULL,
    [Url]             VARCHAR (200)    NULL,
    [IsDefault]       BIT              NULL,
    [IsActive]        BIT              NULL,
    [UpdateHash]      VARBINARY (16)   NULL,
    CONSTRAINT [PK_vVendorAddresses] PRIMARY KEY NONCLUSTERED ([VendorAddressId] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);




GO
CREATE NONCLUSTERED INDEX [vVendorAddress_VendorIdTypeCode_IDX]
    ON [dbo].[vVendorAddresses]([VendorId] ASC, [TypeCode] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE CLUSTERED INDEX [vVendorAddress_VendorIdTypeCodeDetailId_CLSTRD_IDX]
    ON [dbo].[vVendorAddresses]([VendorId] ASC, [TypeCode] ASC, [DetailId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



