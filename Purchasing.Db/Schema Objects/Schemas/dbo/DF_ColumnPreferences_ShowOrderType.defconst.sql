ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPreferences_ShowOrderType] DEFAULT ((0)) FOR [ShowOrderType];

