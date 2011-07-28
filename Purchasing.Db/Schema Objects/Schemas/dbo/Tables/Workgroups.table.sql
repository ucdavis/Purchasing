CREATE TABLE [dbo].[Workgroups] (
    [Id]             INT          IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (50) NOT NULL,
    [OrganizationId] CHAR (4)     NOT NULL,
    [IsActive]       BIT          NOT NULL
);



