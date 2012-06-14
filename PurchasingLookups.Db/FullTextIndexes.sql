CREATE FULLTEXT INDEX ON [dbo].[vAccounts]
    ([Id] LANGUAGE 1033, [Name] LANGUAGE 1033)
    KEY INDEX [vAccounts_Id_UDX]
    ON [vAccounts_IdName_SDX];


GO
CREATE FULLTEXT INDEX ON [dbo].[vBuildings]
    ([BuildingCode] LANGUAGE 1033, [BuildingName] LANGUAGE 1033)
    KEY INDEX [vBuildings_Id_UDX]
    ON [vBuildings_BuildingCodeBuildingName_SDX];


GO
CREATE FULLTEXT INDEX ON [dbo].[vVendors]
    ([Id] LANGUAGE 1033, [Name] LANGUAGE 1033)
    KEY INDEX [vVendors_Id_UDX]
    ON [vVendors_Name_SDX];


GO
CREATE FULLTEXT INDEX ON [dbo].[vCommodities]
    ([Id] LANGUAGE 1033, [Name] LANGUAGE 1033)
    KEY INDEX [vCommodities_Id_UDX]
    ON [vCommodities_IdName_SDX];

