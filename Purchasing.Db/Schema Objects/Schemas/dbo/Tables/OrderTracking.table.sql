CREATE TABLE [dbo].[OrderTracking] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [Description]   VARCHAR (MAX) NOT NULL,
    [OrderId]       INT           NOT NULL,
    [DateCreated]   DATETIME2 (7) NOT NULL,
    [UserId]        VARCHAR (10)  NOT NULL,
    [OrderStatusId] CHAR (2)      NOT NULL
);



