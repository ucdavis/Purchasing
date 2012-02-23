ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPrefrences_ShowStatus] DEFAULT ((0)) FOR [ShowStatus];

