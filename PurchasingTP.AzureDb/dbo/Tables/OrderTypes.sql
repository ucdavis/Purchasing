CREATE TABLE [dbo].[OrderTypes] (
    [Id]                  CHAR (3)     NOT NULL,
    [Name]                VARCHAR (50) NOT NULL,
    [PurchaserAssignable] BIT          DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

