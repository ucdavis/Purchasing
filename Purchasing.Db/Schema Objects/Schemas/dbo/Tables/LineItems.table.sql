CREATE TABLE [dbo].[LineItems] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [Quantity]         DECIMAL (18, 3) NOT NULL,
    [QuantityReceived] DECIMAL (18, 3) NULL,
    [CatalogNumber]    VARCHAR (25)    NULL,
    [Description]      VARCHAR (MAX)   NOT NULL,
    [Unit]             VARCHAR (25)    NULL,
    [UnitPrice]        MONEY           NOT NULL,
    [Url]              VARCHAR (200)   NULL,
    [Notes]            VARCHAR (MAX)   NULL,
    [OrderId]          INT             NOT NULL,
    [CommodityId]      VARCHAR (9)     NULL
);











