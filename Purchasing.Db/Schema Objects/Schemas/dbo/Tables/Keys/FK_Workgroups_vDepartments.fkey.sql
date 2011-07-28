ALTER TABLE [dbo].[Workgroups]
    ADD CONSTRAINT [FK_Workgroups_vDepartments] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[vOrganizations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;



