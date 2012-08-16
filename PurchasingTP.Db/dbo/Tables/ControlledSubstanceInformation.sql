﻿CREATE TABLE [dbo].[ControlledSubstanceInformation] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [ClassSchedule] VARCHAR (10)  NOT NULL,
    [Use]           VARCHAR (200) NOT NULL,
    [StorageSite]   VARCHAR (50)  NOT NULL,
    [Custodian]     VARCHAR (200) NOT NULL,
    [EndUser]       VARCHAR (200) NOT NULL,
    [OrderId]       INT           NOT NULL,
    [PharmaceuticalGrade] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_AuthorizationNumbers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AuthorizationNumbers_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);

