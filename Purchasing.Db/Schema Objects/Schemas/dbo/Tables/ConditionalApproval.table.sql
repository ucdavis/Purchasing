CREATE TABLE [dbo].[ConditionalApproval] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Question]    VARCHAR (100) NOT NULL,
    [ParentId]    INT           NULL,
    [UserId]      VARCHAR (10)  NOT NULL,
    [WorkgroupId] INT           NOT NULL
);

