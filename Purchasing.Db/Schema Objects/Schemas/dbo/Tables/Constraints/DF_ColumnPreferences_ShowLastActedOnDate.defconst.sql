ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPreferences_ShowLastActedOnDate] DEFAULT ((0)) FOR [ShowLastActedOnDate];

