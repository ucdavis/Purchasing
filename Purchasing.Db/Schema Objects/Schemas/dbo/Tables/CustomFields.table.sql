CREATE TABLE [dbo].[CustomFields] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (MAX) NOT NULL,
    [OrganizationId] VARCHAR (10)      NOT NULL,
    [Rank]           INT           NOT NULL,
    [IsActive]       BIT           NOT NULL,
    [IsRequired]     BIT           NOT NULL
);



