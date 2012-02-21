CREATE TABLE [dbo].[ConditionalApproval] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [Question]            VARCHAR (MAX) NOT NULL,
    [PrimaryApproverId]   VARCHAR (10)  NOT NULL,
    [SecondaryApproverId] VARCHAR (10)  NULL,
    [WorkgroupId]         INT           NULL,
    [OrganizationId]      VARCHAR (10)      NULL
);



