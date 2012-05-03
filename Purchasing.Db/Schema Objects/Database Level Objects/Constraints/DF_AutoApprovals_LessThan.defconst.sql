ALTER TABLE [dbo].[AutoApprovals]
    ADD CONSTRAINT [DF_AutoApprovals_LessThan] DEFAULT ((0)) FOR [LessThan];

