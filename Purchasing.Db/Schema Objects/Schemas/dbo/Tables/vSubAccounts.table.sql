CREATE TABLE [dbo].[vSubAccounts] (
    [id]               UNIQUEIDENTIFIER NOT NULL,
    [AccountNumber]    VARCHAR (10)     NOT NULL,
    [SubAccountNumber] VARCHAR (5)      NOT NULL,
    [Name]             VARCHAR (40)     NULL,
    [IsActive]         BIT              NOT NULL,
    [PartitionColumn]  INT              NOT NULL
) ON [EvenOddPartitionScheme] ([PartitionColumn]);



