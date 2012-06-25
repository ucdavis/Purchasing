CREATE TABLE [dbo].[vAccounts] (
    [Id]                      VARCHAR (10) NOT NULL,
    [Name]                    VARCHAR (50) NOT NULL,
    [IsActive]                BIT          CONSTRAINT [DF_vAccounts_IsAct_22AA2996] DEFAULT ((0)) NOT NULL,
    [AccountManager]          VARCHAR (30) NULL,
    [AccountManagerId]        VARCHAR (10) NULL,
    [PrincipalInvestigator]   VARCHAR (30) NULL,
    [PrincipalInvestigatorId] VARCHAR (10) NULL,
    [OrganizationId]          VARCHAR (10) NOT NULL,
    CONSTRAINT [PK_vAccounts] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF),
    CONSTRAINT [FK_vAccounts_vOrganizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[vOrganizations] ([Id])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [vAccounts_Id_UDX]
    ON [dbo].[vAccounts]([Id] ASC)
    ON [PRIMARY];

