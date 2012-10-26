CREATE TABLE [dbo].[ControlledSubstanceInformation] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [ClassSchedule]       VARCHAR (10)  NOT NULL,
    [Use]                 VARCHAR (200) NOT NULL,
    [StorageSite]         VARCHAR (50)  NOT NULL,
    [Custodian]           VARCHAR (200) NOT NULL,
    [EndUser]             VARCHAR (200) NOT NULL,
    [OrderId]             INT           NOT NULL,
    [PharmaceuticalGrade] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AuthorizationNumbers_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);





GO
CREATE NONCLUSTERED INDEX [ControlledSubstanceInformation_OrderId_IDX]
    ON [dbo].[ControlledSubstanceInformation]([OrderId] ASC);

