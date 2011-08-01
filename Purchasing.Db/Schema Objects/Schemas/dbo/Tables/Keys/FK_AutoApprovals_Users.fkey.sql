ALTER TABLE [dbo].[AutoApprovals]
    ADD CONSTRAINT [FK_AutoApprovals_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

