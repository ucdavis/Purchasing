ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPreferences_ShowAccountAndSubAccount] DEFAULT ((0)) FOR [ShowAccountAndSubAccount];

