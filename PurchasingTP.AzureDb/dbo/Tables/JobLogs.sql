﻿CREATE TABLE [dbo].[JobLogs]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(100) NOT NULL, 
    [DateTime] DATETIME NOT NULL DEFAULT getdate(), 
    [Comments] VARCHAR(MAX) NULL
)