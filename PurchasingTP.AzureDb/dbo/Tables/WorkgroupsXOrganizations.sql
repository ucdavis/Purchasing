CREATE TABLE [dbo].[WorkgroupsXOrganizations] (
    [WorkgroupId]    INT          NOT NULL,
    [OrganizationId] VARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([WorkgroupId] ASC, [OrganizationId] ASC),
    CONSTRAINT [FK_WorkgroupsXOrganizations_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [WorkgroupsXOrganizations_WorkgroupId_IDX]
    ON [dbo].[WorkgroupsXOrganizations]([WorkgroupId] ASC);


GO
CREATE NONCLUSTERED INDEX [WorkgroupsXOrgainzations_OrganizationId_IDX]
    ON [dbo].[WorkgroupsXOrganizations]([OrganizationId] ASC);

