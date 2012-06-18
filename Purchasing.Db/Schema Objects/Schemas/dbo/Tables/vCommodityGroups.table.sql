CREATE TABLE [dbo].[vCommodityGroups] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_vCommodityGroups_Id] DEFAULT (newid()) NOT NULL,
    [GroupCode]    VARCHAR (4)      NOT NULL,
    [Name]         VARCHAR (40)     NOT NULL,
    [SubGroupCode] VARCHAR (2)      NOT NULL,
    [SubGroupName] VARCHAR (40)     NOT NULL,
    CONSTRAINT [PK_CommodityGroups] PRIMARY KEY CLUSTERED ([Id] ASC)
);






GO


