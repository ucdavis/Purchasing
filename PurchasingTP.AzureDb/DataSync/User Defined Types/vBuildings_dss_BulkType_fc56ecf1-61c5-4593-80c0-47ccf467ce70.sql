CREATE TYPE [DataSync].[vBuildings_dss_BulkType_fc56ecf1-61c5-4593-80c0-47ccf467ce70] AS TABLE (
    [Id]                         VARCHAR (10) NULL,
    [CampusCode]                 VARCHAR (2)  NOT NULL,
    [BuildingCode]               VARCHAR (5)  NOT NULL,
    [CampusName]                 VARCHAR (40) NULL,
    [CampusShortName]            VARCHAR (12) NULL,
    [CampusTypeCode]             VARCHAR (1)  NULL,
    [BuildingName]               VARCHAR (80) NULL,
    [LastUpdateDate]             DATETIME     NULL,
    [IsActive]                   BIT          NULL,
    [sync_update_peer_timestamp] BIGINT       NULL,
    [sync_update_peer_key]       INT          NULL,
    [sync_create_peer_timestamp] BIGINT       NULL,
    [sync_create_peer_key]       INT          NULL,
    PRIMARY KEY CLUSTERED ([CampusCode] ASC, [BuildingCode] ASC));

