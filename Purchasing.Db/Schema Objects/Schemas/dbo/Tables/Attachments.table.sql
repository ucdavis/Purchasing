CREATE TABLE [dbo].[Attachments] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Filename]    VARCHAR (100)   NOT NULL,
    [ContentType] VARCHAR (200)   NOT NULL,
    [Contents]    VARBINARY (MAX) NOT NULL,
    [OrderId]     INT             NOT NULL,
    [DateCreated] DATETIME        NOT NULL,
    [UserId]      VARCHAR (10)    NOT NULL
);



