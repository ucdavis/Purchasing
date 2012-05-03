ALTER TABLE [dbo].[Attachments]
    ADD CONSTRAINT [DF_Attachments_DateCreated] DEFAULT (getdate()) FOR [DateCreated];

