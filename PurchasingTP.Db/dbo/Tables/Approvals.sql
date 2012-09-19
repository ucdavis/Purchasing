CREATE TABLE [dbo].[Approvals] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [UserId]            VARCHAR (10) NULL,
    [SecondaryUserId]   VARCHAR (10) NULL,
    [Completed]         BIT          CONSTRAINT [DF_Approvals_Approved] DEFAULT ((0)) NOT NULL,
    [OrderStatusCodeId] CHAR (2)     NOT NULL,
    [OrderId]           INT          NOT NULL,
    [SplitId]           INT          NULL,
    CONSTRAINT [PK_Approvals_1] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF),
    CONSTRAINT [FK_Approvals_OrderStatusCodes] FOREIGN KEY ([OrderStatusCodeId]) REFERENCES [dbo].[OrderStatusCodes] ([Id]),
    CONSTRAINT [FK_Approvals_Splits] FOREIGN KEY ([SplitId]) REFERENCES [dbo].[Splits] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [Approvals_UserId_IDX]
    ON [dbo].[Approvals]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [Approvals_SecondaryUserId_IDX]
    ON [dbo].[Approvals]([SecondaryUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [Approvals_OrderStatusCodeId_IDX]
    ON [dbo].[Approvals]([OrderStatusCodeId] ASC);


GO
CREATE NONCLUSTERED INDEX [Approvals_orderid_IDX]
    ON [dbo].[Approvals]([OrderId] ASC);


GO
CREATE NONCLUSTERED INDEX [Approvals_Completed_IDX]
    ON [dbo].[Approvals]([Completed] ASC);


GO
CREATE NONCLUSTERED INDEX [HistoryReceivedLineItems_UserID_IDX]
    ON [dbo].[Approvals]([SplitId] ASC);


GO
CREATE NONCLUSTERED INDEX [Approvals_SplitId_IDX]
    ON [dbo].[Approvals]([SplitId] ASC);

