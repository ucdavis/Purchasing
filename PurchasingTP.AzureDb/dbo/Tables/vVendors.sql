CREATE TABLE [dbo].[vVendors] (
    [Id]               VARCHAR (10)   NOT NULL,
    [Name]             VARCHAR (45)   NULL,
    [OwnershipCode]    VARCHAR (2)    NULL,
    [BusinessTypeCode] VARCHAR (4)    NULL,
    [IsActive]         BIT            NULL,
    [UpdateHash]       VARBINARY (16) NULL,
    CONSTRAINT [PK_vVendors] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [vVendors_Id_UDX] UNIQUE NONCLUSTERED ([Id] ASC)
);

