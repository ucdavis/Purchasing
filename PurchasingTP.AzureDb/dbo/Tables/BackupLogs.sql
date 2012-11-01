CREATE TABLE [dbo].[BackupLogs]
(
	[Id] INT NOT NULL IDENTITY,
	RequestId varchar(max) NOT NULL,
	[DateTimeCreated]	datetime default getdate(),
	Completed bit default 0,
	[Filename] VARCHAR(MAX) NOT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [DateTimeDeleted] DATETIME NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
