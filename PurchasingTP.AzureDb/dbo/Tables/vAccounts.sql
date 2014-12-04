CREATE TABLE [dbo].[vAccounts] (
    [Id]                      VARCHAR (10)   NOT NULL,
    [Name]                    VARCHAR (50)   NOT NULL,
    [IsActive]                BIT            DEFAULT ((0)) NOT NULL,
    [AccountManager]          VARCHAR (30)   NULL,
    [AccountManagerId]        VARCHAR (10)   NULL,
    [PrincipalInvestigator]   VARCHAR (30)   NULL,
    [PrincipalInvestigatorId] VARCHAR (10)   NULL,
    [OrganizationId]          VARCHAR (10)   NOT NULL,
    [UpdateHash]              VARBINARY (16) NULL,
    CONSTRAINT [PK_vAccounts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [vAccounts_Id_UDX] UNIQUE NONCLUSTERED ([Id] ASC)
);

