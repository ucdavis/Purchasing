CREATE TABLE [dbo].[vOrganizationTypes] (
    [Id]       VARCHAR (4)  NOT NULL,
    [Name]     VARCHAR (50) NOT NULL,
    [IsActive] BIT          DEFAULT ((1)) NOT NULL
);

