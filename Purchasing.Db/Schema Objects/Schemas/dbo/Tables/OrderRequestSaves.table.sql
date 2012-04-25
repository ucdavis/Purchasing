CREATE TABLE [dbo].[OrderRequestSaves] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Name]         VARCHAR (150)    NOT NULL,
    [UserId]       VARCHAR (10)     NOT NULL,
    [PreparedById] VARCHAR (10)     NULL,
    [WorkgroupId]  INT              NOT NULL,
    [FormData]     VARCHAR (MAX)    NOT NULL,
    [AccountData]  VARCHAR (MAX)    NOT NULL,
    [DateCreated]  DATETIME         NOT NULL,
    [LastUpdate]   DATETIME         NOT NULL,
    [Version]      VARCHAR (15)     NULL
);





