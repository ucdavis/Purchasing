ALTER TABLE [dbo].[vCommodityGroups]
    ADD CONSTRAINT [DF_vCommodityGroups_Id] DEFAULT (newid()) FOR [Id];

