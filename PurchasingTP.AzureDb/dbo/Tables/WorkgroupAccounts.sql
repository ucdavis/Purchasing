CREATE TABLE [dbo].[WorkgroupAccounts] (
    [Id]                   INT          IDENTITY (1, 1) NOT NULL,
    [AccountId]            VARCHAR (10) NOT NULL,
    [WorkgroupId]          INT          NOT NULL,
    [ApproverUserId]       VARCHAR (10) NULL,
    [AccountManagerUserId] VARCHAR (10) NULL,
    [PurchaserUserId]      VARCHAR (10) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_WorkgroupAccounts_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [WorkgroupAccounts_WorkgroupId_IDX]
    ON [dbo].[WorkgroupAccounts]([WorkgroupId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



