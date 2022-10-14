CREATE TABLE [dbo].[idxBefore] (
    [object_id]                              INT            NOT NULL,
    [ObjectSchema]                           NVARCHAR (128) NULL,
    [ObjectName]                             NVARCHAR (128) NULL,
    [IndexName]                              [sysname]      NULL,
    [type]                                   TINYINT        NOT NULL,
    [type_desc]                              NVARCHAR (60)  COLLATE Latin1_General_CI_AS_KS_WS NULL,
    [avg_fragmentation_in_percent]           FLOAT (53)     NULL,
    [page_count]                             BIGINT         NULL,
    [index_id]                               INT            NULL,
    [partition_number]                       INT            NULL,
    [avg_page_space_used_in_percent]         FLOAT (53)     NULL,
    [record_count]                           BIGINT         NULL,
    [ghost_record_count]                     BIGINT         NULL,
    [forwarded_record_count]                 BIGINT         NULL,
    [OnlineOpIsNotSupported]                 INT            NULL,
    [ObjectDoesNotSupportResumableOperation] INT            NULL,
    [SkipIndex]                              INT            NOT NULL,
    [SkipReason]                             VARCHAR (128)  NULL
);

