CREATE TABLE [dbo].[Workgroups] (
    [Id]                        INT           IDENTITY (1, 1) NOT NULL,
    [Name]                      VARCHAR (50)  NOT NULL,
    [PrimaryOrganizationId]     VARCHAR (10)  NOT NULL,
    [IsActive]                  BIT           CONSTRAINT [DF__Workgroup__IsAct__239E4DCF] DEFAULT ((1)) NOT NULL,
    [Administrative]            BIT           CONSTRAINT [DF__Workgroup__Admin__1AD3FDA4] DEFAULT ((0)) NOT NULL,
    [SharedOrCluster]           BIT           CONSTRAINT [DF_Workgroups_SharedOrCluster] DEFAULT ((0)) NOT NULL,
    [Disclaimer]                VARCHAR (MAX) NULL,
    [SyncAccounts]              BIT           CONSTRAINT [DF__Workgroup__SyncA__416EA7D8] DEFAULT ((0)) NOT NULL,
    [AllowControlledSubstances] BIT           CONSTRAINT [DF_Workgroups_AllowControlledSubstances] DEFAULT ((0)) NOT NULL,
    [ForceAccountApprover]      BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Workgroups_1] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF)
);


GO
CREATE NONCLUSTERED INDEX [Workgroups_SharedOrCluster_IDX]
    ON [dbo].[Workgroups]([SharedOrCluster] ASC);


GO
CREATE NONCLUSTERED INDEX [Workgroups_IsActive_IDX]
    ON [dbo].[Workgroups]([IsActive] ASC);


GO
CREATE NONCLUSTERED INDEX [Workgroups_Administrative_IDX]
    ON [dbo].[Workgroups]([Administrative] ASC);

