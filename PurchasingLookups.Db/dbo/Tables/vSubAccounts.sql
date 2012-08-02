CREATE TABLE [dbo].[vSubAccounts] (
    [id]               UNIQUEIDENTIFIER CONSTRAINT [DF_vSubAccounts_id] DEFAULT (newid()) NOT NULL,
    [AccountNumber]    VARCHAR (10)     NOT NULL,
    [SubAccountNumber] VARCHAR (5)      NOT NULL,
    [Name]             VARCHAR (40)     NULL,
    [IsActive]         BIT              NOT NULL,
    CONSTRAINT [PK_vSubAccounts] PRIMARY KEY CLUSTERED ([AccountNumber] ASC, [SubAccountNumber] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF)
);



