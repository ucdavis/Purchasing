CREATE TABLE [dbo].[UsersXOrganizations] (
    [UserId]         VARCHAR (10) NOT NULL,
    [OrganizationId] VARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC, [OrganizationId] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);







GO
CREATE NONCLUSTERED INDEX [UsersXOrganizations_UserId_IDX]
    ON [dbo].[UsersXOrganizations]([UserId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



