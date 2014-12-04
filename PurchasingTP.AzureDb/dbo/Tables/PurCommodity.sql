CREATE TABLE [dbo].[PurCommodity] (
    [Id]           VARCHAR (40)   NOT NULL,
    [Name]         VARCHAR (200)  NOT NULL,
    [GroupCode]    VARCHAR (4)    NULL,
    [SubGroupCode] VARCHAR (2)    NULL,
    [isActive]     INT            NOT NULL,
    [UpdateHash]   VARBINARY (16) NULL,
    CONSTRAINT [PK_PurCommodity] PRIMARY KEY NONCLUSTERED ([Id] ASC, [isActive] ASC)
);


GO
CREATE CLUSTERED INDEX [PurCommodity_Id_CLUSTRD_IDX]
    ON [dbo].[PurCommodity]([Id] ASC, [isActive] ASC);

