ALTER TABLE [dbo].[ServiceMessages]
    ADD CONSTRAINT [DF_ServiceMessages_Critical] DEFAULT ((0)) FOR [Critical];

