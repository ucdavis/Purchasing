CREATE TABLE [dbo].[WorkgroupSyncLogs]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [WorkgroupId] INT NOT NULL, 
    [UserId] VARCHAR(10) NOT NULL, 
    [RoleId] CHAR(2) NULL, 
    [Action] CHAR(1) NOT NULL, 
    [Message] VARCHAR(250) NULL, 
    [ActionDate] DATETIME NOT NULL, 
    [SyncKeyUpdate] BIT NOT NULL DEFAULT ((0))
)
