ALTER TABLE [dbo].[EmailPreferences]
    ADD CONSTRAINT [DF_EmailPreferences_PurchaserOrderArrive] DEFAULT ((1)) FOR [PurchaserOrderArrive];

