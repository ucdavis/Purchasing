CREATE TABLE [dbo].[WorkgroupAccounts] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [AccountId]   VARCHAR (10) NOT NULL,
    [WorkgroupId] INT          NOT NULL
);

