CREATE TABLE [dbo].[LineItems] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [Quantity]         DECIMAL (18, 3) NOT NULL,
    [QuantityReceived] DECIMAL (18, 3) NULL,
    [CatalogNumber]    VARCHAR (50)    NULL,
    [Description]      VARCHAR (MAX)   NOT NULL,
    [Unit]             VARCHAR (25)    NULL,
    [UnitPrice]        MONEY           NOT NULL,
    [Url]              VARCHAR (2000)  NULL,
    [Notes]            VARCHAR (MAX)   NULL,
    [OrderId]          INT             NOT NULL,
    [CommodityId]      VARCHAR (9)     NULL,
    [Received]         AS              (case when ([Quantity]-[QuantityReceived])<=(0) then CONVERT([bit],(1),(0)) else CONVERT([bit],(0),(0)) end) PERSISTED,
	[Paid]             AS              (case when ([Quantity]-[QuantityPaid])<=(0) then CONVERT([bit],(1),(0)) else CONVERT([bit],(0),(0)) end) PERSISTED,
    [ReceivedNotes]    VARCHAR (MAX)   NULL,
	[QuantityPaid] DECIMAL(18, 3) NULL, 
    [PaidNotes] VARCHAR(MAX) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LineItems_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [LineItems_OrderId_IDX]
    ON [dbo].[LineItems]([OrderId] ASC);

