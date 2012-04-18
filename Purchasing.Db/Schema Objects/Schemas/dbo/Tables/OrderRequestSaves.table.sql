CREATE TABLE [dbo].[OrderRequestSaves] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [UserId]      VARCHAR (10)     NOT NULL,
    [PreparedBy]  VARCHAR (10)     NULL,
    [FormData]    VARCHAR (MAX)    NOT NULL,
    [DateCreated] DATETIME         NOT NULL,
    [LastUpdate]  DATETIME         NOT NULL,
    [Version]     VARCHAR (15)     NULL
);

