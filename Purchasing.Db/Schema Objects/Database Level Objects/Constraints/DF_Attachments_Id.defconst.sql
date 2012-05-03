ALTER TABLE [dbo].[Attachments]
    ADD CONSTRAINT [DF_Attachments_Id] DEFAULT (newid()) FOR [Id];

