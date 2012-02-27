ALTER TABLE [dbo].[Workgroups]
    ADD CONSTRAINT [DF_Workgroups_AllowControlledSubstances] DEFAULT ((0)) FOR [AllowControlledSubstances];

