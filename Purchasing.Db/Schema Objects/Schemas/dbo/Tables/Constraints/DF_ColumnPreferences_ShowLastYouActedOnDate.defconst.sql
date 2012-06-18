ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPreferences_ShowLastYouActedOnDate] DEFAULT ((0)) FOR [ShowLastYouActedOnDate];

