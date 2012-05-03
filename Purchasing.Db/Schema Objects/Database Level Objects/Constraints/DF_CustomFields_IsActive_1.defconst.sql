ALTER TABLE [dbo].[CustomFields]
    ADD CONSTRAINT [DF_CustomFields_IsActive] DEFAULT ((1)) FOR [IsActive];

