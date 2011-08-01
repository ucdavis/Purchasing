ALTER TABLE [dbo].[WorkgroupAddresses]
    ADD CONSTRAINT [FK_WorkgroupAddresses_States] FOREIGN KEY ([StateId]) REFERENCES [dbo].[States] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

