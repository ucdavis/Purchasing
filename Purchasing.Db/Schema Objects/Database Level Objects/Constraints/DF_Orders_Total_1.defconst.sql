ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [DF_Orders_Total] DEFAULT ((0)) FOR [Total];

