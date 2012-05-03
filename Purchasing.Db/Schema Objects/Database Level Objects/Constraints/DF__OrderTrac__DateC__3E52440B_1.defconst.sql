ALTER TABLE [dbo].[OrderTracking]
    ADD CONSTRAINT [DF__OrderTrac__DateC__3E52440B] DEFAULT (getdate()) FOR [DateCreated];

