CREATE TABLE [dbo].[Favorites] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [UserId]      VARCHAR (10)  NOT NULL,
    [OrderId]     INT           NOT NULL,
    [CreatedDate] DATETIME      NOT NULL,
    [Category]    VARCHAR (50)  NULL,
    [Notes]       VARCHAR (MAX) NULL,
    [IsActive]    BIT           CONSTRAINT [DF_Favorites_IsActive] DEFAULT ((1)) NOT NULL
);

