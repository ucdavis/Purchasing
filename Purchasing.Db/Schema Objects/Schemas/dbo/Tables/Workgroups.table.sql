CREATE TABLE [dbo].[Workgroups] (
	[Id]                    INT          IDENTITY (1, 1) NOT NULL,
	[Name]                  VARCHAR (50) NOT NULL,
	[PrimaryOrganizationId] CHAR (4)     NOT NULL,
	[IsActive]              BIT          NOT NULL,
	[Administrative]		BIT			 DEFAULT 0 NOT NULL,
	[Disclaimer]			varchar(max) NULL
);







