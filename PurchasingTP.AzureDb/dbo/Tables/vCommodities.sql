CREATE TABLE [dbo].[vCommodities] (
    [Id]           VARCHAR (40)   NOT NULL,
    [Name]         VARCHAR (200)  NOT NULL,
    [GroupCode]    VARCHAR (4)    NULL,
    [SubGroupCode] VARCHAR (2)    NULL,
    [IsActive]     BIT            NOT NULL,
    [UpdateHash]   VARBINARY (16) NULL,
    CONSTRAINT [PK_vCommodities] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [vCommodities_Id_UDX] UNIQUE NONCLUSTERED ([Id] ASC)
);

