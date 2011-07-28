ALTER TABLE [dbo].[vAccounts]
    ADD CONSTRAINT [FK_vAccounts_vDepartments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[vOrganizations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;



