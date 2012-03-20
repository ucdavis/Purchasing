CREATE TABLE [dbo].[ServiceMessages] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [Message]          VARCHAR (MAX) NOT NULL,
    [BeginDisplayDate] DATETIME2 (7) NOT NULL,
    [EndDisplayDate]   DATETIME2 (7) NULL,
    [Critical]         BIT           NOT NULL,
    [IsActive]         BIT           NOT NULL
);

