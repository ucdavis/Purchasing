ALTER TABLE [dbo].[CustomFieldAnswers]
    ADD CONSTRAINT [FK_CustomFieldAnswers_CustomFields] FOREIGN KEY ([CustomFieldId]) REFERENCES [dbo].[CustomFields] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

