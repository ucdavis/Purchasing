CREATE TABLE [dbo].[BackupLogs]
(
	[Id] INT NOT NULL IDENTITY,
	RequestId varchar(max) NOT NULL,
	[DateTimeCreated]	datetime default getdate(),
	Completed bit default 0,
	PRIMARY KEY CLUSTERED ([Id] ASC)
)
