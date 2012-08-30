CREATE TABLE [dbo].[vReadAccessTmp]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
	[OrderId] int not null,
    [AccessUserId] VARCHAR(10) NOT NULL, 
	[ReadAccess] bit not null DEFAULT 1,
	[EditAccess] bit not null default (0),
	[IsAdmin] bit NOT NULL,
    [AccessLevel] CHAR(2) NOT NULL
)
