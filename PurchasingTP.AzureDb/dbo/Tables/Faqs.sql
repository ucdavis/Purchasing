CREATE TABLE [dbo].[Faqs] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Category] VARCHAR (50)  NOT NULL,
    [Question] VARCHAR (MAX) NOT NULL,
    [Answer]   VARCHAR (MAX) NOT NULL,
    [OrgId]    VARCHAR (10)  NULL,
    [Like]     INT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

