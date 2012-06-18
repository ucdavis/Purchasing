ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPrefrences_ShowDaysNotActedOn] DEFAULT ((0)) FOR [ShowDaysNotActedOn];

