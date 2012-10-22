CREATE TABLE [dbo].[ServiceMessages] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [Message]          VARCHAR (MAX) NOT NULL,
    [BeginDisplayDate] DATETIME2 (7) CONSTRAINT [DF_ServiceMessages_BeginDisplayDate] DEFAULT (getdate()) NOT NULL,
    [EndDisplayDate]   DATETIME2 (7) NULL,
    [Critical]         BIT           CONSTRAINT [DF_ServiceMessages_Critical] DEFAULT ((0)) NOT NULL,
    [IsActive]         BIT           CONSTRAINT [DF_ServiceMessages_IsActive] DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

