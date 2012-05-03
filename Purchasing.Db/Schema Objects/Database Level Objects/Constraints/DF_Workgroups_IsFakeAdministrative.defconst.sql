ALTER TABLE [dbo].[Workgroups]
    ADD CONSTRAINT [DF_Workgroups_SharedOrCluster] DEFAULT ((0)) FOR [SharedOrCluster];

