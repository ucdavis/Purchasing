CREATE TABLE [dbo].[Workgroups] (
    [Id]                        INT           IDENTITY (1, 1) NOT NULL,
    [Name]                      VARCHAR (50)  NOT NULL,
    [PrimaryOrganizationId]     VARCHAR (10)  NOT NULL,
    [IsActive]                  BIT           NOT NULL,
    [Administrative]            BIT           NOT NULL,
    [SharedOrCluster]	        BIT           NOT NULL,
    [Disclaimer]                VARCHAR (MAX) NULL,
    [SyncAccounts]              BIT           NOT NULL,
    [AllowControlledSubstances] BIT           NOT NULL
);













