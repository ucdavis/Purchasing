ALTER TABLE [dbo].[ConditionalApproval]
    ADD CONSTRAINT [FK_ConditionalApproval_Users] FOREIGN KEY ([PrimaryApproverId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

