CREATE TABLE [dbo].[vSubAccounts] (
    [id]               UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [AccountNumber]    VARCHAR (10)     NOT NULL,
    [SubAccountNumber] VARCHAR (5)      NOT NULL,
    [Name]             VARCHAR (40)     NULL,
    [IsActive]         BIT              NOT NULL,
    [UpdateHash]       VARBINARY (16)   NULL,
    CONSTRAINT [PK_vSubAccounts] PRIMARY KEY CLUSTERED ([AccountNumber] ASC, [SubAccountNumber] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);






GO
CREATE NONCLUSTERED INDEX [nci_wi_vSubAccounts_D5E67DA673835CC5DC09051A0338E175]
    ON [dbo].[vSubAccounts]([IsActive] ASC)
    INCLUDE([AccountNumber], [Name], [SubAccountNumber]);

