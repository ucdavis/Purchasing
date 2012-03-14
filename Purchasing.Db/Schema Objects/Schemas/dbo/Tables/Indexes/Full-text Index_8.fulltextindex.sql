CREATE FULLTEXT INDEX ON [dbo].[vBuildings]
    ([BuildingCode] LANGUAGE 1033, [BuildingName] LANGUAGE 1033)
    KEY INDEX [vBuildings_Id_UDX]
    ON [vBuildings_BuildingCodeBuildingName_SDX];

