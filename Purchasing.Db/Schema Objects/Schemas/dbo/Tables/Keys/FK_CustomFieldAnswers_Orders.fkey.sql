ALTER TABLE [dbo].[CustomFieldAnswers]
    ADD CONSTRAINT [FK_CustomFieldAnswers_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

