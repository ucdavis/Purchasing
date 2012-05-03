ALTER TABLE [dbo].[EmailPreferences]
    ADD CONSTRAINT [DF_EmailPreferences_AccountManagerOrderArrive] DEFAULT ((1)) FOR [AccountManagerOrderArrive];

