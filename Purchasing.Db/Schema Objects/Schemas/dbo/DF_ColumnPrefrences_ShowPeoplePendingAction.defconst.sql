ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPrefrences_ShowPeoplePendingAction] DEFAULT ((0)) FOR [ShowPeoplePendingAction];

