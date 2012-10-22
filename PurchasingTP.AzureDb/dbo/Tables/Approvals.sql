CREATE TABLE [dbo].[Approvals] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [UserId]            VARCHAR (10) NULL,
    [SecondaryUserId]   VARCHAR (10) NULL,
    [Completed]         BIT          CONSTRAINT [DF_Approvals_Approved] DEFAULT ((0)) NOT NULL,
    [OrderStatusCodeId] CHAR (2)     NOT NULL,
    [OrderId]           INT          NOT NULL,
    [SplitId]           INT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Approvals_OrderStatusCodes] FOREIGN KEY ([OrderStatusCodeId]) REFERENCES [dbo].[OrderStatusCodes] ([Id]),
    CONSTRAINT [FK_Approvals_Splits] FOREIGN KEY ([SplitId]) REFERENCES [dbo].[Splits] ([Id])
);