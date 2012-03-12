CREATE TABLE [dbo].[vBuildings] (
    [Id]              VARCHAR (10) NOT NULL,
    [CampusCode]      VARCHAR (2)  NOT NULL,
    [BuildingCode]    VARCHAR (4)  NOT NULL,
    [CampusName]      VARCHAR (40) NULL,
    [CampusShortName] VARCHAR (12) NULL,
    [CampusTypeCode]  VARCHAR (1)  NULL,
    [BuildingName]    VARCHAR (80) NULL,
    [LastUpdateDate]  DATETIME     NULL,
    [IsActive]        BIT          NULL
);



