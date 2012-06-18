CREATE TABLE [dbo].[vCommodities] (
    [Id]              VARCHAR (9)  NOT NULL,
    [Name]            VARCHAR (60) NOT NULL,
    [GroupCode]       VARCHAR (4)  NULL,
    [SubGroupCode]    VARCHAR (2)  NULL,
    [IsActive]        BIT          NOT NULL,
    [PartitionColumn] INT          NOT NULL,
    CONSTRAINT [PK_vCommodities] PRIMARY KEY CLUSTERED ([Id] ASC, [PartitionColumn] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF) ON [EvenOddPartitionScheme] ([PartitionColumn])
);






GO


