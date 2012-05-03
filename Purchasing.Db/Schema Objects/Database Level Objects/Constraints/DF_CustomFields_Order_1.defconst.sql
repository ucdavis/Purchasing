ALTER TABLE [dbo].[CustomFields]
    ADD CONSTRAINT [DF_CustomFields_Order] DEFAULT ((0)) FOR [Rank];

