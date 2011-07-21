CREATE TABLE [dbo].[Workgroups] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50) NOT NULL,
    [DepartmentId] VARCHAR (6)  NOT NULL,
    [IsActive]     BIT          DEFAULT ((1)) NOT NULL
);

