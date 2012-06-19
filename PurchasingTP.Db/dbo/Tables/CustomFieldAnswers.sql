CREATE TABLE [dbo].[CustomFieldAnswers] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [CustomFieldId] INT           NOT NULL,
    [Answer]        VARCHAR (MAX) NOT NULL,
    [OrderId]       INT           NOT NULL,
    CONSTRAINT [PK_CustomFieldAnswers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomFieldAnswers_CustomFields] FOREIGN KEY ([CustomFieldId]) REFERENCES [dbo].[CustomFields] ([Id]),
    CONSTRAINT [FK_CustomFieldAnswers_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);

