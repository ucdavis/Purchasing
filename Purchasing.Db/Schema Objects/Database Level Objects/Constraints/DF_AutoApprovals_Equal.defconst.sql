ALTER TABLE [dbo].[AutoApprovals]
    ADD CONSTRAINT [DF_AutoApprovals_Equal] DEFAULT ((0)) FOR [Equal];

