CREATE TABLE [dbo].[WorkgroupAccountPermissions] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [UserId]             VARCHAR (10) NOT NULL,
    [RoleId]             CHAR (2)     NOT NULL,
    [WorkgroupAccountId] INT          NOT NULL
);

