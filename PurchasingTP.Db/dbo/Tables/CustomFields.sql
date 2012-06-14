CREATE TABLE [dbo].[CustomFields] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (MAX) NOT NULL,
    [OrganizationId] VARCHAR (10)  NOT NULL,
    [Rank]           INT           CONSTRAINT [DF_CustomFields_Order] DEFAULT ((0)) NOT NULL,
    [IsActive]       BIT           CONSTRAINT [DF_CustomFields_IsActive] DEFAULT ((1)) NOT NULL,
    [IsRequired]     BIT           CONSTRAINT [DF_CustomFields_IsRequired] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CustomFields] PRIMARY KEY CLUSTERED ([Id] ASC)
);

