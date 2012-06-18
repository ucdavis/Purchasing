CREATE TABLE [dbo].[BugTracking]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OrderId] INT NOT NULL, 
    [UserId] VARCHAR(20) NOT NULL, 
    [DateTimeStamp] DATETIME NOT NULL, 
    [TrackingMessage] VARCHAR(500) NULL, 
    [SplitId] INT NULL, 
    [LineItemId] INT NULL
)
