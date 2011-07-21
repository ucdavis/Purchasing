ALTER TABLE [dbo].[vAccounts]
    ADD CONSTRAINT [FK_vAccounts_vDepartments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[vDepartments] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

