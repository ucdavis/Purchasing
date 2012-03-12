CREATE TABLE [dbo].[WorkgroupAddresses] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)  NOT NULL,
    [Building]     VARCHAR (50)  NULL,
    [BuildingCode] VARCHAR (10)  NULL,
    [Room]         VARCHAR (50)  NULL,
    [Address]      VARCHAR (100) NOT NULL,
    [City]         VARCHAR (100) NOT NULL,
    [StateId]      CHAR (2)      NOT NULL,
    [Zip]          VARCHAR (10)  NOT NULL,
    [Phone]        VARCHAR (15)  NULL,
    [WorkgroupId]  INT           NOT NULL,
    [IsActive]     BIT           NOT NULL
);





