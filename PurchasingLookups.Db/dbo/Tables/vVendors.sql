CREATE TABLE [dbo].[vVendors] (
    [Id]               VARCHAR (10) NOT NULL,
    [Name]             VARCHAR (45) NOT NULL,
    [OwnershipCode]    VARCHAR (2)  NULL,
    [BusinessTypeCode] VARCHAR (4)  NULL,
    [IsActive]         BIT          NULL,
    CONSTRAINT [PK_vVendors] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [vVendors_Id_UDX]
    ON [dbo].[vVendors]([Id] ASC)
    ON [PRIMARY];

