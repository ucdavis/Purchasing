ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPrefrences_ShowOrganization] DEFAULT ((0)) FOR [ShowOrganization];

