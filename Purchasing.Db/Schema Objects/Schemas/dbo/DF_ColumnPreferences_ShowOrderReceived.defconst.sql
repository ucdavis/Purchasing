ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPreferences_ShowOrderReceived] DEFAULT ((0)) FOR [ShowOrderReceived];