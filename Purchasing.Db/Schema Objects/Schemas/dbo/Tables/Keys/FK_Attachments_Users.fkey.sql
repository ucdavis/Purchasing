ALTER TABLE [dbo].[Attachments]
    ADD CONSTRAINT [FK_Attachments_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

