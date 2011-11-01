ALTER TABLE [dbo].[Approvals]
    ADD CONSTRAINT [DF_Approvals_Approved] DEFAULT ((0)) FOR [Completed];

