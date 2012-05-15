
GO
CREATE FULLTEXT INDEX ON [dbo].[vAccounts]
    ([Id] LANGUAGE 1033, [Name] LANGUAGE 1033)
    KEY INDEX [vAccounts_Id_UDX]
    ON [vAccounts_IdName_SDX];

