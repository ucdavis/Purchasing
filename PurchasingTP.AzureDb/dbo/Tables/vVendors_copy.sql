CREATE TABLE [dbo].[vVendors_copy] (
    [Id]               VARCHAR (10)   NOT NULL,
    [Name]             VARCHAR (45)   NULL,
    [OwnershipCode]    VARCHAR (2)    NULL,
    [BusinessTypeCode] VARCHAR (2)    NULL,
    [IsActive]         BIT            NULL,
    [UpdateHash]       VARBINARY (16) NULL,
    CONSTRAINT [PK_vVendors_copy] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [vVendors_copy_Id_UDX] UNIQUE NONCLUSTERED ([Id] ASC)
);

