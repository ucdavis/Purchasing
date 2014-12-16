CREATE TABLE [dbo].[vVendorAddresses_sync] (
    [Id]             BIGINT         NOT NULL,
    [VendorId]       CHAR (10)      NOT NULL,
    [TypeCode]       VARCHAR (4)    NOT NULL,
    [AddressType]    VARCHAR (4)    NULL,
    [Name]           VARCHAR (50)   NULL,
    [Line1]          VARCHAR (45)   NULL,
    [Line2]          VARCHAR (45)   NULL,
    [Line3]          VARCHAR (45)   NULL,
    [City]           VARCHAR (45)   NULL,
    [State]          CHAR (2)       NULL,
    [Zip]            VARCHAR (20)   NULL,
    [CountryCode]    VARCHAR (2)    NULL,
    [PhoneNumber]    VARCHAR (40)   NULL,
    [FaxNumber]      VARCHAR (40)   NULL,
    [Email]          VARCHAR (90)   NULL,
    [Url]            VARCHAR (200)  NULL,
    [IsDefault]      BIT            NULL,
    [IsActive]       BIT            NULL,
    [LastUpdateDate] DATETIME2 (7)  NULL,
    [UpdateHash]     VARBINARY (16) NULL,
    [DetailId]       VARCHAR (4)    NULL,
    CONSTRAINT [PK_vVendorAddresses_sync] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [vVendorAddresses_Id_CLUS_IDX]
    ON [dbo].[vVendorAddresses_sync]([Id] ASC);

