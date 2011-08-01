CREATE TABLE [dbo].[AutoApprovals] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [TargetUserId] VARCHAR (10) NULL,
    [AccountId]    VARCHAR (10) NULL,
    [MaxAmount]    MONEY        NOT NULL,
    [LessThan]     BIT          NOT NULL,
    [Equal]        BIT          NOT NULL,
    [IsActive]     BIT          NOT NULL,
    [UserId]       VARCHAR (10) NOT NULL
);

