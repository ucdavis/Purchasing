ALTER TABLE [dbo].[Approvals]
    ADD CONSTRAINT [FK_Approvals_Splits] FOREIGN KEY ([SplitId]) REFERENCES [dbo].[Splits] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

