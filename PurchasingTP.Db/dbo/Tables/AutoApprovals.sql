CREATE TABLE [dbo].[AutoApprovals] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [TargetUserId] VARCHAR (10) NULL,
    [AccountId]    VARCHAR (10) NULL,
    [MaxAmount]    MONEY        NOT NULL,
    [LessThan]     BIT          CONSTRAINT [DF_AutoApprovals_LessThan] DEFAULT ((0)) NOT NULL,
    [Equal]        BIT          CONSTRAINT [DF_AutoApprovals_Equal] DEFAULT ((0)) NOT NULL,
    [IsActive]     BIT          CONSTRAINT [DF_AutoApprovals_IsActive] DEFAULT ((0)) NOT NULL,
    [UserId]       VARCHAR (10) NOT NULL,
    [Expiration]   DATE         NULL,
    CONSTRAINT [PK_AutoApprovals] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AutoApprovals_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_AutoApprovals_Users1] FOREIGN KEY ([TargetUserId]) REFERENCES [dbo].[Users] ([Id])
);

