CREATE TABLE [dbo].[vAccounts] (
    [Id]                      VARCHAR (10) NOT NULL,
    [Name]                    VARCHAR (50) NOT NULL,
    [IsActive]                BIT           NOT NULL,
    [AccountManager]          VARCHAR (30) NULL,
    [AccountManagerId]        VARCHAR (10) NULL,
    [PrincipalInvestigator]   VARCHAR (30) NULL,
    [PrincipalInvestigatorId] VARCHAR (10) NULL,
    [OrganizationId]          VARCHAR (10) NOT NULL,
    [PartitionColumn]         INT          NOT NULL,
    CONSTRAINT [PK_vAccounts] PRIMARY KEY CLUSTERED ([Id] ASC, [PartitionColumn] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF) ON [EvenOddPartitionScheme] ([PartitionColumn]),
    CONSTRAINT [FK_vAccounts_vOrganizations] FOREIGN KEY ([OrganizationId], [PartitionColumn]) REFERENCES [dbo].[vOrganizations] ([Id], [PartitionColumn])
);














GO
CREATE UNIQUE NONCLUSTERED INDEX [vAccounts_Id_UDX]
    ON [dbo].[vAccounts]([Id] ASC)
    ON [PRIMARY];

