ALTER TABLE [dbo].[Approvals]
    ADD CONSTRAINT [FK_Approvals_OrderStatusCodes] FOREIGN KEY ([OrderStatusCodeId]) REFERENCES [dbo].[OrderStatusCodes] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

