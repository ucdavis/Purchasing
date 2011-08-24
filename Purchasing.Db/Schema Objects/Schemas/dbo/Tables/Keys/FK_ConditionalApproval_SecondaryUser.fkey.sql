ALTER TABLE [dbo].[ConditionalApproval]
    ADD CONSTRAINT [FK_ConditionalApproval_SecondaryUser] FOREIGN KEY ([SecondaryApproverId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

