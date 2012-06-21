CREATE TABLE [dbo].[DepartmentalAdminRequests]
(
	[Id] VARCHAR(10) NOT NULL PRIMARY KEY, 
    [FirstName] VARCHAR(50) NOT NULL, 
    [LastName] VARCHAR(50) NOT NULL, 
    [Email] VARCHAR(50) NOT NULL, 
    [PhoneNumber] VARCHAR(50) NULL, 
    [DepartmentSize] TINYINT NOT NULL, 
    [SharedOrCluster] BIT NOT NULL, 
    [Complete] BIT NOT NULL DEFAULT 0, 
    [DateCreated] DATETIME NOT NULL, 
    [Organizations] VARCHAR(MAX) NOT NULL, 
    [RequestCount] INT NOT NULL DEFAULT 0 
)
