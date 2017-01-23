CREATE TABLE [dbo].[vOrganizationDescendants] (
    [id]                INT          IDENTITY (1, 1) NOT NULL,
    [OrgId]             VARCHAR (10) NOT NULL,
    [Name]              VARCHAR (50) NOT NULL,
    [IsActive]          BIT          DEFAULT ((1)) NOT NULL,
    [ImmediateParentId] VARCHAR (10) NULL,
    [RollupParentId]    VARCHAR (10) NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [nci_wi_vOrganizationDescendants_6992A67F3973348D2691C195903D58FD]
    ON [dbo].[vOrganizationDescendants]([RollupParentId] ASC);


GO
CREATE CLUSTERED INDEX [nci_wi_vOrganizationDescendants_A9356AAAD0088C1A44FB244F7B890EEA]
    ON [dbo].[vOrganizationDescendants]([OrgId] ASC, [RollupParentId] ASC);

