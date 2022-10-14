CREATE TABLE [dbo].[statsBefore] (
    [ObjectSchema]         NVARCHAR (128) NULL,
    [ObjectName]           NVARCHAR (128) NULL,
    [object_id]            INT            NOT NULL,
    [stats_id]             INT            NOT NULL,
    [StatsName]            NVARCHAR (128) NULL,
    [last_updated]         DATETIME2 (7)  NULL,
    [rows]                 BIGINT         NULL,
    [rows_sampled]         BIGINT         NULL,
    [modification_counter] BIGINT         NULL,
    [type]                 TINYINT        NULL,
    [type_desc]            NVARCHAR (60)  COLLATE Latin1_General_CI_AS_KS_WS NULL,
    [SkipStatistics]       INT            NOT NULL
);

