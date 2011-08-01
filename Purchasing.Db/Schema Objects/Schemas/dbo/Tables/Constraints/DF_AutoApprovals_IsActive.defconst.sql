ALTER TABLE [dbo].[AutoApprovals]
    ADD CONSTRAINT [DF_AutoApprovals_IsActive] DEFAULT ((0)) FOR [IsActive];

