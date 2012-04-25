ALTER TABLE [dbo].[OrderRequestSaves]
    ADD CONSTRAINT [FK_OrderRequestSaves_Users1] FOREIGN KEY ([PreparedById]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;



