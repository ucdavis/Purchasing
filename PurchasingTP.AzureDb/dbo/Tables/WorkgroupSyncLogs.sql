CREATE TABLE [dbo].[WorkgroupSyncLog]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [WorkgroupId] INT NOT NULL, 
    [UserId] VARCHAR(10) NOT NULL, 
    [RoleId] CHAR(2) NULL, 
    [Action] CHAR(1) NOT NULL, 
    [Message] VARCHAR(250) NULL, 
    [ActionDate] DATETIME NOT NULL
)
