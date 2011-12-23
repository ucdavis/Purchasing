CREATE TABLE [dbo].[CustomFieldAnswers] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [CustomFieldId] INT           NOT NULL,
    [Answer]        VARCHAR (MAX) NOT NULL,
    [OrderId]       INT           NOT NULL
);

