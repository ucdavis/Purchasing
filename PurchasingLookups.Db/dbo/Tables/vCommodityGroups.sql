CREATE TABLE [dbo].[vCommodityGroups] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_vCommodityGroups_Id] DEFAULT (newid()) NOT NULL,
    [GroupCode]    VARCHAR (4)      NOT NULL,
    [Name]         VARCHAR (40)     NOT NULL,
    [SubGroupCode] VARCHAR (2)      NOT NULL,
    [SubGroupName] VARCHAR (40)     NOT NULL,
    CONSTRAINT [PK_vCommodityGroups] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF)
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Commodity group code', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vCommodityGroups', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Commodity group code', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vCommodityGroups', @level2type = N'COLUMN', @level2name = N'GroupCode';

