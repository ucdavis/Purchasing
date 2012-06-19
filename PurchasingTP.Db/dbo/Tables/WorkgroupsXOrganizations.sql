CREATE TABLE [dbo].[WorkgroupsXOrganizations] (
    [WorkgroupId]    INT          NOT NULL,
    [OrganizationId] VARCHAR (10) NOT NULL,
    CONSTRAINT [PK_WorkgroupsXOrganizations] PRIMARY KEY CLUSTERED ([WorkgroupId] ASC, [OrganizationId] ASC),
    CONSTRAINT [FK_WorkgroupsXOrganizations_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);

