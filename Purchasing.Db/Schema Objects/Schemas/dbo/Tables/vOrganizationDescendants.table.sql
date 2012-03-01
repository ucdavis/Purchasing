CREATE TABLE [dbo].[vOrganizationDescendants] (
	[id]                INT          IDENTITY (1, 1) NOT NULL,
	[OrgId]             VARCHAR (10) NOT NULL,
	[Name]              VARCHAR (50) NOT NULL,
	[IsActive]			BIT			 NOT NULL DEFAULT 1,
	[ImmediateParentId] VARCHAR (10) NULL,
	[RollupParentId]    VARCHAR (10) NULL
);

