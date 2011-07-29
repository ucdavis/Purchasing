CREATE TABLE [dbo].[vAccounts] (
    [Id]                    VARCHAR (10) NOT NULL,
    [Name]                  VARCHAR (50) NOT NULL,
    [IsActive]              BIT          NOT NULL,
    [AccountManager]        VARCHAR (30) NOT NULL,
    [PrincipalInvestigator] VARCHAR (30) NULL,
    [OrganizationId]        CHAR (4)     NOT NULL
);







