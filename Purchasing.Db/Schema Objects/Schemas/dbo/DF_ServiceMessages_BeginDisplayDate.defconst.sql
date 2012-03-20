ALTER TABLE [dbo].[ServiceMessages]
    ADD CONSTRAINT [DF_ServiceMessages_BeginDisplayDate] DEFAULT (getdate()) FOR [BeginDisplayDate];

