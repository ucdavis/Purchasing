CREATE TABLE [dbo].[vVendors] (
    [Id]               VARCHAR (10) NOT NULL,
    [Name]             VARCHAR (40) NOT NULL,
    [OwnershipCode]    VARCHAR (2)  NULL,
    [BusinessTypeCode] VARCHAR (2)  NULL,
    [IsActive]         BIT          NULL,
    [PartitionColumn]  INT          NOT NULL
) ON [EvenOddPartitionScheme] ([PartitionColumn]);





