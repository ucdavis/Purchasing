CREATE TABLE [dbo].[vOrganizationDescendants] (
    [id]                INT          IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [OrgId]             VARCHAR (10) NOT NULL,
    [Name]              VARCHAR (50) NOT NULL,
    [IsActive]          BIT          DEFAULT ((1)) NOT NULL,
    [ImmediateParentId] VARCHAR (10) NULL,
    [RollupParentId]    VARCHAR (10) NULL,
);




GO
CREATE CLUSTERED INDEX [vOrganizationsDescendants_OrgIdRollupParentId_IDX]
    ON [dbo].[vOrganizationDescendants]([OrgId] ASC, [RollupParentId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_vOrgainzationsDescendants]
    ON [dbo].[vOrganizationDescendants]([id] ASC);

