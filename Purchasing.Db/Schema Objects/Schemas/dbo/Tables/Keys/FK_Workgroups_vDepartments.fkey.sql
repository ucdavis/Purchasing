ALTER TABLE [dbo].[Workgroups]
    ADD CONSTRAINT [FK_Workgroups_vDepartments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[vDepartments] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

