CREATE TABLE [dbo].[Approvals] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [UserId]            VARCHAR (10) NULL,
    [SecondaryUserId]   VARCHAR (10) NULL,
    [Completed]         BIT          CONSTRAINT [DF_Approvals_Approved] DEFAULT ((0)) NOT NULL,
    [OrderStatusCodeId] CHAR (2)     NOT NULL,
    [OrderId]           INT          NOT NULL,
    [SplitId]           INT          NULL,
    [IsExternal]        BIT          DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_Approvals_OrderStatusCodes] FOREIGN KEY ([OrderStatusCodeId]) REFERENCES [dbo].[OrderStatusCodes] ([Id]),
    CONSTRAINT [FK_Approvals_Splits] FOREIGN KEY ([SplitId]) REFERENCES [dbo].[Splits] ([Id])
);








GO
CREATE NONCLUSTERED INDEX [HistoryReceivedLineItems_UserID_IDX]
    ON [dbo].[Approvals]([SplitId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_UserId_IDX]
    ON [dbo].[Approvals]([UserId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_SplitId_IDX]
    ON [dbo].[Approvals]([SplitId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_SecondaryUserId_IDX]
    ON [dbo].[Approvals]([SecondaryUserId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_OrderStatusCodeId_IDX]
    ON [dbo].[Approvals]([OrderStatusCodeId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_orderid_IDX]
    ON [dbo].[Approvals]([OrderId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_Completed_IDX]
    ON [dbo].[Approvals]([Completed] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvials_Completed_Incl_UserIdOrderStatusCodeIdOrderId_CVIDX]
    ON [dbo].[Approvals]([Completed] ASC)
    INCLUDE([UserId], [OrderStatusCodeId], [OrderId]) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_UserIdCompletedOrderStatusCodeId_Incl_OrderId_CVIDX]
    ON [dbo].[Approvals]([UserId] ASC, [Completed] ASC, [OrderStatusCodeId] ASC)
    INCLUDE([OrderId]) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_SecondaryUserIdCompletedOrderStatusCodeId_Incl_OrderId_CVIDX]
    ON [dbo].[Approvals]([SecondaryUserId] ASC, [Completed] ASC, [OrderStatusCodeId] ASC)
    INCLUDE([OrderId]) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_OrderStatusCodeId_Incl_UserIdSecondaryUserIdOrderId_CVIDX]
    ON [dbo].[Approvals]([OrderStatusCodeId] ASC)
    INCLUDE([UserId], [SecondaryUserId], [OrderId]) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Approvals_OrderStatusCodeId_Incl_IdUserIdSecondaryUserIdCompletedOrderId_CVINDX]
    ON [dbo].[Approvals]([OrderStatusCodeId] ASC)
    INCLUDE([Id], [UserId], [SecondaryUserId], [Completed], [OrderId]) WITH (STATISTICS_NORECOMPUTE = ON);



