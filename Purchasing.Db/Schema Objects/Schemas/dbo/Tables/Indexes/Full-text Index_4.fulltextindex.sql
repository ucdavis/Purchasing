CREATE FULLTEXT INDEX ON [dbo].[vCommodities]
    ([Name] LANGUAGE 1033, [Id] LANGUAGE 1033)
    KEY INDEX [vCommodities_Id_UDX]
    ON [vCommodities_IdName_SDX];

