ALTER TABLE [dbo].[vVendorAddresses]
    ADD CONSTRAINT [DF_vVendorAddresses_Id] DEFAULT (newid()) FOR [Id];

