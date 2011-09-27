CREATE TABLE [dbo].[Approvals] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [UserId]            VARCHAR (10) NULL,
    [Approved]          BIT          NULL,
    [OrderStatusCodeId] CHAR (2)     NOT NULL,
    [OrderId]           INT          NOT NULL
);







