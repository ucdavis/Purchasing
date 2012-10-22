CREATE TABLE [dbo].[vOrganizationDescendants] (
    [id]                INT          IDENTITY (1, 1) NOT NULL,
    [OrgId]             VARCHAR (10) NOT NULL,
    [Name]              VARCHAR (50) NOT NULL,
    [IsActive]          BIT          DEFAULT ((1)) NOT NULL,
    [ImmediateParentId] VARCHAR (10) NULL,
    [RollupParentId]    VARCHAR (10) NULL,
	PRIMARY KEY CLUSTERED ([ID] ASC)
);