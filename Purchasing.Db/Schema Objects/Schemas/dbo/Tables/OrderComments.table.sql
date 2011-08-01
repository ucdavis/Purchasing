CREATE TABLE [dbo].[OrderComments] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Text]        VARCHAR (MAX) NOT NULL,
    [DateCreated] DATETIME      NOT NULL,
    [UserId]      VARCHAR (10)  NOT NULL,
    [OrderId]     INT           NOT NULL
);

