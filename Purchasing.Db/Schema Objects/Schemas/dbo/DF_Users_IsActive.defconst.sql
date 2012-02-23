ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [DF_Users_IsActive] DEFAULT ((1)) FOR [IsActive];

