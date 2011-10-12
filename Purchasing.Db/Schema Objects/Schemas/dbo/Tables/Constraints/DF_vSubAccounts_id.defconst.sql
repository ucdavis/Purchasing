ALTER TABLE [dbo].[vSubAccounts]
    ADD CONSTRAINT [DF_vSubAccounts_id] DEFAULT (newid()) FOR [id];

