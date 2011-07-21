CREATE TABLE [dbo].[vAccounts] (
    [Id]             VARCHAR (10) NOT NULL,
    [Name]           VARCHAR (50) NOT NULL,
    [IsActive]       BIT          DEFAULT ((0)) NOT NULL,
    [AccountManager] VARCHAR (15) NOT NULL,
    [PI]             VARCHAR (15) NULL,
    [DepartmentId]   VARCHAR (6)  NOT NULL
);

