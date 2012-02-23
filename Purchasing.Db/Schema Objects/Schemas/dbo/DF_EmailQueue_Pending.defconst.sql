ALTER TABLE [dbo].[EmailQueue]
    ADD CONSTRAINT [DF_EmailQueue_Pending] DEFAULT ((1)) FOR [Pending];

