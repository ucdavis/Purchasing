CREATE TABLE [dbo].[Approvals] (
    [Id]             INT          IDENTITY (1, 1) NOT NULL,
    [Level]          INT          NOT NULL,
    [UserId]         VARCHAR (10) NULL,
    [Approved]       BIT          NULL,
    [ApprovalTypeId] CHAR (2)     NOT NULL
);

