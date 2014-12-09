CREATE TYPE [DataSync].[vCommodities_dss_BulkType_fc56ecf1-61c5-4593-80c0-47ccf467ce70] AS TABLE (
    [Id]                         VARCHAR (40)  NOT NULL,
    [Name]                       VARCHAR (200) NULL,
    [GroupCode]                  VARCHAR (4)   NULL,
    [SubGroupCode]               VARCHAR (2)   NULL,
    [IsActive]                   BIT           NULL,
    [sync_update_peer_timestamp] BIGINT        NULL,
    [sync_update_peer_key]       INT           NULL,
    [sync_create_peer_timestamp] BIGINT        NULL,
    [sync_create_peer_key]       INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC));

