ALTER TABLE [dbo].[OrderStatusCodes]
    ADD CONSTRAINT [DF_OrderStatusCodes_KfsStatus] DEFAULT ((0)) FOR [KfsStatus];

