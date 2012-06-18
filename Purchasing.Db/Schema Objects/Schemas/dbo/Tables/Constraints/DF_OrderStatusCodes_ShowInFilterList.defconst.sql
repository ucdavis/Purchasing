ALTER TABLE [dbo].[OrderStatusCodes]
    ADD CONSTRAINT [DF_OrderStatusCodes_ShowInFilterList] DEFAULT ((1)) FOR [ShowInFilterList];

