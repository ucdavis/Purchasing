CREATE TABLE [dbo].[Roles] (
    [Id]    CHAR (2)     NOT NULL,
    [Name]  VARCHAR (50) NOT NULL,
    [Level] INT          NOT NULL, 
    [IsAdmin] BIT NOT NULL DEFAULT 0
);



