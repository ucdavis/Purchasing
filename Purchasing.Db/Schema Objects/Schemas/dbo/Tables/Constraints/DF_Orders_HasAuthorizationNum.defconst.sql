ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [DF_Orders_HasAuthorizationNum] DEFAULT ((0)) FOR [HasAuthorizationNum];

