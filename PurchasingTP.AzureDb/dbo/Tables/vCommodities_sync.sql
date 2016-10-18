CREATE TABLE [dbo].[vCommodities_sync] (
    [Id]           VARCHAR (40)   NOT NULL,
    [Name]         VARCHAR (200)  NOT NULL,
    [GroupCode]    VARCHAR (4)    NULL,
    [SubGroupCode] VARCHAR (2)    NULL,
    [IsActive]     BIT            NOT NULL,
    [UpdateHash]   VARBINARY (16) NULL,
    CONSTRAINT [PK_vCommodities_temp] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [vCommodities_temp_Id_UDX] UNIQUE NONCLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);



