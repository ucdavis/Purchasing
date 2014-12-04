CREATE TABLE [dbo].[vCommodityGroups_sync] (
    [Id]           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [GroupCode]    VARCHAR (4)      NOT NULL,
    [Name]         VARCHAR (40)     NOT NULL,
    [SubGroupCode] VARCHAR (2)      NOT NULL,
    [SubGroupName] VARCHAR (40)     NOT NULL,
    [IsActive]     BIT              NULL,
    [UpdateHash]   VARBINARY (16)   NULL,
    CONSTRAINT [PK_vCommodityGroups_sync] PRIMARY KEY CLUSTERED ([Id] ASC)
);

