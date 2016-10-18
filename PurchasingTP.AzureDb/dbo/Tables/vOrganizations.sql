CREATE TABLE [dbo].[vOrganizations] (
    [Id]         VARCHAR (10)   NOT NULL,
    [Name]       VARCHAR (50)   NOT NULL,
    [TypeCode]   CHAR (1)       NOT NULL,
    [TypeName]   VARCHAR (50)   NOT NULL,
    [ParentId]   VARCHAR (10)   NULL,
    [IsActive]   BIT            NOT NULL,
    [UpdateHash] VARBINARY (16) NULL,
    CONSTRAINT [PK_vOrganizations] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [vOrganizations_Id_UDX] UNIQUE NONCLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);



