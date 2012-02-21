CREATE TABLE [dbo].[Workgroups] (
	[Id]                    INT          IDENTITY (1, 1) NOT NULL,
	[Name]                  VARCHAR (50) NOT NULL,
	[PrimaryOrganizationId] VARCHAR (10)     NOT NULL,
	[IsActive]              BIT          NOT NULL,
	[Administrative]		BIT			 DEFAULT 0 NOT NULL,
	[Disclaimer]			varchar(max) NULL,
	[SyncAccounts]			BIT			 DEFAULT 0 NOT NULL
);







