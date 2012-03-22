CREATE FULLTEXT INDEX ON [dbo].[vVendors]
    ([Id] LANGUAGE 1033, [Name] LANGUAGE 1033)
    KEY INDEX [vVendors_Id_UDX]
    ON [vVendors_Name_SDX];



