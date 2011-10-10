ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [DF_Orders_DateCreated] DEFAULT (getdate()) FOR [DateCreated];

