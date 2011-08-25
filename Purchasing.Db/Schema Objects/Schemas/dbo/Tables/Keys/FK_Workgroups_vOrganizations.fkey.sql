ALTER TABLE [dbo].[Workgroups]
    ADD CONSTRAINT [FK_Workgroups_vOrganizations] FOREIGN KEY ([PrimaryOrganizationId]) REFERENCES [dbo].[vOrganizations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

