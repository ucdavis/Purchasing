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