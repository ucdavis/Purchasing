ALTER TABLE [dbo].[EmailPreferences]
    ADD CONSTRAINT [FK_EmailPreferences_Users] FOREIGN KEY ([Id]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

