ALTER TABLE [dbo].[WorkgroupsXOrganizations]
    ADD CONSTRAINT [FK_WorkgroupsXOrganizations_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

