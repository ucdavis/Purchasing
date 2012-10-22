CREATE TABLE [dbo].[Audits] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [ObjectName]  VARCHAR (50)     NOT NULL,
    [ObjectId]    VARCHAR (50)     NULL,
    [AuditAction] CHAR (1)         NOT NULL,
    [Username]    NVARCHAR (256)   NOT NULL,
    [AuditDate]   DATETIME         NOT NULL,
    CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED ([ID] ASC)
);

