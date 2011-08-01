ALTER TABLE [dbo].[Attachments]
    ADD CONSTRAINT [FK_Attachments_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

