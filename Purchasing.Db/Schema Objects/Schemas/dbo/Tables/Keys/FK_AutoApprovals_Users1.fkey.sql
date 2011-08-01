ALTER TABLE [dbo].[AutoApprovals]
    ADD CONSTRAINT [FK_AutoApprovals_Users1] FOREIGN KEY ([TargetUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

