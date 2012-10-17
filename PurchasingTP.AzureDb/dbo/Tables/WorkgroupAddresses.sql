CREATE TABLE [dbo].[WorkgroupAddresses] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)  NOT NULL,
    [Building]     VARCHAR (50)  NULL,
    [BuildingCode] VARCHAR (10)  NULL,
    [Room]         VARCHAR (50)  NULL,
    [Address]      VARCHAR (100) NOT NULL,
    [City]         VARCHAR (100) NOT NULL,
    [StateId]      CHAR (2)      NOT NULL,
    [Zip]          VARCHAR (10)  NOT NULL,
    [Phone]        VARCHAR (15)  NULL,
    [WorkgroupId]  INT           NOT NULL,
    [IsActive]     BIT           CONSTRAINT [DF_WorkgroupAddresses_IsActive] DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkgroupAddresses_States] FOREIGN KEY ([StateId]) REFERENCES [dbo].[States] ([Id]),
    CONSTRAINT [FK_WorkgroupAddresses_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [WorkgroupAddresses_StateId_IDX]
    ON [dbo].[WorkgroupAddresses]([StateId] ASC);


GO
CREATE NONCLUSTERED INDEX [WorkgroupAddresses_WorkgroupId_IDX]
    ON [dbo].[WorkgroupAddresses]([WorkgroupId] ASC);

