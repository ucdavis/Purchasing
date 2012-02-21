CREATE TABLE [dbo].[vOrganizations] (
    [Id]              VARCHAR (10) NOT NULL,
    [Name]            VARCHAR (50) NOT NULL,
    [TypeCode]        CHAR (1)     NOT NULL,
    [TypeName]        VARCHAR (50) NOT NULL,
    [ParentId]        VARCHAR (10) NULL,
    [IsActive]        BIT          NOT NULL,
    [PartitionColumn] INT          NOT NULL
) ON [EvenOddPartitionScheme] ([PartitionColumn]);



