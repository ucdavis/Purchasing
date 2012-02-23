ALTER TABLE [dbo].[CustomFields]
    ADD CONSTRAINT [FK_CustomFields_vOrganizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[vOrganizations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

