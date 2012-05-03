ALTER TABLE [dbo].[ParamNameAndValue]
    ADD CONSTRAINT [DF_ParamNameAndValue_IsActive] DEFAULT ((1)) FOR [IsActive];

