ALTER TABLE [dbo].[EmailQueue]
    ADD CONSTRAINT [DF_EmailQueue_DateTimeCreated] DEFAULT (getdate()) FOR [DateTimeCreated];

