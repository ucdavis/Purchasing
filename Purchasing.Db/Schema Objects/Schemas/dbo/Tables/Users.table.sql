CREATE TABLE [dbo].[Users] (
    [Id]        VARCHAR (10) NOT NULL,
    [FirstName] VARCHAR (50) NOT NULL,
    [LastName]  VARCHAR (50) NOT NULL,
    [Email]     VARCHAR (50) NOT NULL,
    [AwayUntil] DATETIME     NULL,
    [IsActive]  BIT          NOT NULL,
    [IsAway]    AS           (isnull(CONVERT([bit],datediff(day,[awayuntil],getdate()),0),(0)))
);







