CREATE TABLE [dbo].[UsersXOrganizations] (
    [UserId]         VARCHAR (10) NOT NULL,
    [OrganizationId] VARCHAR (10) NOT NULL,
    CONSTRAINT [PK_UsersXOrganizations] PRIMARY KEY CLUSTERED ([UserId] ASC, [OrganizationId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [UsersXOrganizations_UserId_IDX]
    ON [dbo].[UsersXOrganizations]([UserId] ASC);

