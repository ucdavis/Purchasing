﻿CREATE TABLE [dbo].[ShippingTypes] (
    [Id]      CHAR (2)      NOT NULL,
    [Name]    VARCHAR (50)  NOT NULL,
    [Warning] VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

