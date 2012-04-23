ALTER TABLE [dbo].[OrderRequestSaves]
    ADD CONSTRAINT [DF_OrderRequestSaves_LastUpdate] DEFAULT (getdate()) FOR [LastUpdate];

