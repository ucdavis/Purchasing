ALTER TABLE [dbo].[WorkgroupVendors]
    ADD CONSTRAINT [DF_WorkgroupVendors_IsActive] DEFAULT ((1)) FOR [IsActive];

