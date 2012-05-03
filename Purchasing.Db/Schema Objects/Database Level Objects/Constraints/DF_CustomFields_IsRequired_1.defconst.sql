ALTER TABLE [dbo].[CustomFields]
    ADD CONSTRAINT [DF_CustomFields_IsRequired] DEFAULT ((0)) FOR [IsRequired];

