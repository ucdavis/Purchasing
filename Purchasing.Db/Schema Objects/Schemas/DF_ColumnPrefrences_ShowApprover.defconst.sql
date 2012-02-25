ALTER TABLE [dbo].[ColumnPreferences]
    ADD CONSTRAINT [DF_ColumnPrefrences_ShowApprover] DEFAULT ((0)) FOR [ShowApprover];

