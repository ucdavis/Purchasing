CREATE TABLE [dbo].[vOrganizations] (
    [Id]         VARCHAR (10)   NOT NULL,
    [Name]       VARCHAR (50)   NOT NULL,
    [TypeCode]   CHAR (1)       NOT NULL,
    [TypeName]   VARCHAR (50)   NOT NULL,
    [ParentId]   VARCHAR (10)   NULL,
    [IsActive]   BIT            NOT NULL,
    [UpdateHash] VARBINARY (16) NULL,
    CONSTRAINT [PK_vOrganizations] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [vOrganizations_Id_UDX] UNIQUE NONCLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);






GO
CREATE CLUSTERED INDEX [ci_wi_vOrganizations_0600A1D7ACC9613C0A1F6DE78CC90F5A]
    ON [dbo].[vOrganizations]([ParentId] ASC, [IsActive] ASC, [Name] ASC);

