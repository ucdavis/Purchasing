CREATE TABLE [dbo].[WorkgroupPermissions] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [WorkgroupId] INT          NOT NULL,
    [UserId]      VARCHAR (10) NOT NULL,
    [RoleTypeId]  CHAR (2)     NOT NULL
);

