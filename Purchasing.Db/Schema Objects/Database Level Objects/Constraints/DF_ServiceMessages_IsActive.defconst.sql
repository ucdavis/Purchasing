ALTER TABLE [dbo].[ServiceMessages]
    ADD CONSTRAINT [DF_ServiceMessages_IsActive] DEFAULT ((1)) FOR [IsActive];

