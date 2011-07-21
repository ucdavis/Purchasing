ALTER TABLE [dbo].[ApprovalsXSplits]
    ADD CONSTRAINT [FK_ApprovalsXSplits_Splits] FOREIGN KEY ([SplitId]) REFERENCES [dbo].[Splits] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

