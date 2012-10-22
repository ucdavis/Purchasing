CREATE TABLE [dbo].[UsersXOrganizations] (
    [UserId]         VARCHAR (10) NOT NULL,
    [OrganizationId] VARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC, [OrganizationId] ASC)
);


