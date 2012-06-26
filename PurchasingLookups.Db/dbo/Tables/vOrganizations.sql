CREATE TABLE [dbo].[vOrganizations] (
    [Id]       VARCHAR (10) NOT NULL,
    [Name]     VARCHAR (50) NOT NULL,
    [TypeCode] CHAR (1)     NOT NULL,
    [TypeName] VARCHAR (50) NOT NULL,
    [ParentId] VARCHAR (10) NULL,
    [IsActive] BIT          NOT NULL,
    CONSTRAINT [PK_vOrganizations] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [vOrganizations_Id_UDX]
    ON [dbo].[vOrganizations]([Id] ASC)
    ON [PRIMARY];

