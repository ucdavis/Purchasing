CREATE TABLE [dbo].[WorkgroupAccounts] (
    [Id]                   INT          IDENTITY (1, 1) NOT NULL,
    [AccountId]            VARCHAR (10) NOT NULL,
    [WorkgroupId]          INT          NOT NULL,
    [ApproverUserId]       VARCHAR (10) NULL,
    [AccountManagerUserId] VARCHAR (10) NULL,
    [PurchaserUserId]      VARCHAR (10) NULL,
    CONSTRAINT [PK_WorkgroupAccounts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkgroupAccounts_vAccounts] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[vAccounts] ([Id]),
    CONSTRAINT [FK_WorkgroupAccounts_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);





