CREATE TABLE [dbo].[vVendors_sync] (
    [Id]               VARCHAR (10)   NOT NULL,
    [Name]             VARCHAR (45)   NULL,
    [VendorTypeCode]   VARCHAR (4)    NULL,
    [OwnershipCode]    VARCHAR (2)    NULL,
    [BusinessTypeCode] VARCHAR (4)    NULL,
    [IsActive]         BIT            NULL,
    [UpdateHash]       VARBINARY (16) NULL,
    CONSTRAINT [PK_vVendors_sync] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [vVendors_sync_Id_UDX]
    ON [dbo].[vVendors_sync]([Id] ASC);

