CREATE TABLE [dbo].[AuthorizationNumbers] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [AuthorizationNum] VARCHAR (10)  NOT NULL,
    [ClassSchedule]    VARCHAR (10)  NOT NULL,
    [Use]              VARCHAR (200) NOT NULL,
    [StorageSite]      VARCHAR (50)  NOT NULL,
    [Custodian]        VARCHAR (200) NOT NULL,
    [EndUser]          VARCHAR (200) NOT NULL,
    [OrderId]          INT           NOT NULL
);

