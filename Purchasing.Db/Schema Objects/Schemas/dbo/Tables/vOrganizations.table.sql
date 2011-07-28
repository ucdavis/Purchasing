CREATE TABLE [dbo].[vOrganizations] (
    [Id]       CHAR (4)     NOT NULL,
    [Name]     VARCHAR (50) NOT NULL,
    [TypeCode] CHAR (1)     NOT NULL,
    [TypeName] VARCHAR (50) NOT NULL,
    [ParentId] CHAR (4)     NULL,
    [IsActive] BIT          NOT NULL
);

