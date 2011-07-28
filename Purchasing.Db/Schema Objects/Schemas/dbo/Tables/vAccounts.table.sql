CREATE TABLE [dbo].[vAccounts] (
    [Id]             VARCHAR (10) NOT NULL,
    [Name]           VARCHAR (50) NOT NULL,
    [IsActive]       BIT          NOT NULL,
    [AccountManager] VARCHAR (15) NOT NULL,
    [PI]             VARCHAR (15) NULL,
    [DepartmentId]   CHAR (4)     NOT NULL
);



