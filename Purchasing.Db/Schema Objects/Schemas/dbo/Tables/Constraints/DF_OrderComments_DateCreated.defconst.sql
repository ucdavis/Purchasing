ALTER TABLE [dbo].[OrderComments]
    ADD CONSTRAINT [DF_OrderComments_DateCreated] DEFAULT (getdate()) FOR [DateCreated];

