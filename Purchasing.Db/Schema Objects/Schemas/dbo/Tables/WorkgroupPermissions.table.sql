CREATE TABLE [dbo].[WorkgroupPermissions] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [WorkgroupId] INT          NOT NULL,
    [UserId]      VARCHAR (10) NOT NULL,
    [RoleId]      CHAR (2)     NOT NULL, 
    [IsAdmin] BIT NOT NULL DEFAULT 0, 
    [ParentWorkgroupId] INT NULL
);



