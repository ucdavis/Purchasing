ALTER TABLE [dbo].[vAccounts]
    ADD CONSTRAINT [FK_vAccounts_vOrganizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[vOrganizations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

