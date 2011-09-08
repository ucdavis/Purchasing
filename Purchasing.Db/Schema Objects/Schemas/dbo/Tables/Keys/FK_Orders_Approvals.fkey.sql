ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_Approvals] FOREIGN KEY ([LastCompletedApprovalId]) REFERENCES [dbo].[Approvals] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

