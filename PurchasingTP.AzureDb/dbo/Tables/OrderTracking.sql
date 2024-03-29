﻿CREATE TABLE [dbo].[OrderTracking] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [Description]       VARCHAR (MAX) NOT NULL,
    [OrderId]           INT           NOT NULL,
    [DateCreated]       DATETIME2 (7) CONSTRAINT [DF__OrderTrac__DateC__3E52440B] DEFAULT (getdate()) NOT NULL,
    [UserId]            VARCHAR (10)  NOT NULL,
    [OrderStatusCodeId] CHAR (2)      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderTracking_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT [FK_OrderTracking_OrderStatusCodes] FOREIGN KEY ([OrderStatusCodeId]) REFERENCES [dbo].[OrderStatusCodes] ([Id]),
    CONSTRAINT [FK_OrderTracking_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);












GO
CREATE NONCLUSTERED INDEX [OrderTracking_UserId_IDX]
    ON [dbo].[OrderTracking]([UserId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [OrderTracking_OrderStatusCodeId_IDX]
    ON [dbo].[OrderTracking]([OrderStatusCodeId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [OrderTracking_OrderId_IDX]
    ON [dbo].[OrderTracking]([OrderId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [OrderTracking_UserId_Incl_OrderIdOrderStatusCodeId_CVIDX]
    ON [dbo].[OrderTracking]([UserId] ASC)
    INCLUDE([OrderId], [OrderStatusCodeId]) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [nci_wi_OrderTracking_B5D42DBEA7740A3E914FD6E95C68E93F]
    ON [dbo].[OrderTracking]([DateCreated] ASC)
    INCLUDE([Description], [OrderId], [UserId]);


GO
CREATE TRIGGER SetOrderDateLastAction
ON OrderTracking
AFTER INSERT
AS
   UPDATE Orders SET DateLastAction = GETUTCDATE() WHERE Id IN (SELECT OrderId from inserted)