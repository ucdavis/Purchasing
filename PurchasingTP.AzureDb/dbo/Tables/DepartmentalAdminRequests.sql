CREATE TABLE [dbo].[DepartmentalAdminRequests] (
    [Id]               VARCHAR (10)  NOT NULL,
    [FirstName]        VARCHAR (50)  NOT NULL,
    [LastName]         VARCHAR (50)  NOT NULL,
    [Email]            VARCHAR (50)  NOT NULL,
    [PhoneNumber]      VARCHAR (50)  NULL,
    [DepartmentSize]   TINYINT       NOT NULL,
    [SharedOrCluster]  BIT           NOT NULL,
    [Complete]         BIT           DEFAULT ((0)) NOT NULL,
    [DateCreated]      DATETIME      NOT NULL,
    [Organizations]    VARCHAR (MAX) NULL,
    [RequestCount]     INT           DEFAULT ((0)) NOT NULL,
    [AttendedTraining] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

