ALTER TABLE [dbo].[EmailPreferences]
    ADD CONSTRAINT [DF_EmailPreferences_ApprovedOrderAssigned] DEFAULT ((1)) FOR [ApproverOrderArrive];



