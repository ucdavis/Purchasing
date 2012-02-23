ALTER TABLE [dbo].[WorkgroupAddresses]
    ADD CONSTRAINT [DF_WorkgroupAddresses_IsActive] DEFAULT ((1)) FOR [IsActive];

