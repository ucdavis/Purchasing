CREATE TABLE [dbo].[OrderStatusCodes] (
    [Id]               CHAR (2)     NOT NULL,
    [Name]             VARCHAR (50) NOT NULL,
    [Level]            INT          NULL,
    [IsComplete]       BIT          NOT NULL,
    [KfsStatus]        BIT          NOT NULL,
    [ShowInFilterList] BIT          NOT NULL
);





