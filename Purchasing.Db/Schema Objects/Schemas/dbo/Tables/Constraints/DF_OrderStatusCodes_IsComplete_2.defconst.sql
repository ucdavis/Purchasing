ALTER TABLE [dbo].[OrderStatusCodes]
    ADD CONSTRAINT [DF_OrderStatusCodes_IsComplete] DEFAULT ((0)) FOR [IsComplete];

