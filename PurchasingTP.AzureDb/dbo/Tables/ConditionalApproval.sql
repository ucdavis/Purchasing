CREATE TABLE [dbo].[ConditionalApproval] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [Question]            VARCHAR (MAX) NOT NULL,
    [PrimaryApproverId]   VARCHAR (10)  NOT NULL,
    [SecondaryApproverId] VARCHAR (10)  NULL,
    [WorkgroupId]         INT           NULL,
    [OrganizationId]      VARCHAR (10)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ConditionalApproval_SecondaryUser] FOREIGN KEY ([SecondaryApproverId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_ConditionalApproval_Users] FOREIGN KEY ([PrimaryApproverId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_ConditionalApproval_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [HistoryReceivedLineItems_UserID_IDX]
    ON [dbo].[ConditionalApproval]([PrimaryApproverId] ASC);


GO
CREATE NONCLUSTERED INDEX [ConditionalApproval_WorkgroupId_IDX]
    ON [dbo].[ConditionalApproval]([WorkgroupId] ASC);


GO
CREATE NONCLUSTERED INDEX [ConditionalApproval_SecondaryApproverId_IDX]
    ON [dbo].[ConditionalApproval]([SecondaryApproverId] ASC);


GO
CREATE NONCLUSTERED INDEX [ConditionalApproval_PrimaryApproverId_IDX]
    ON [dbo].[ConditionalApproval]([PrimaryApproverId] ASC);

