CREATE TABLE [dbo].[vOrganizations_sync] (
    [Id]         VARCHAR (10)   NOT NULL,
    [Name]       VARCHAR (50)   NOT NULL,
    [TypeCode]   CHAR (1)       NOT NULL,
    [TypeName]   VARCHAR (50)   NOT NULL,
    [ParentId]   VARCHAR (10)   NULL,
    [IsActive]   BIT            NOT NULL,
    [UpdateHash] VARBINARY (16) NULL,
    CONSTRAINT [PK_vOrganizations_temp] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [vOrganizations_temp_Id_UDX] UNIQUE NONCLUSTERED ([Id] ASC)
);

