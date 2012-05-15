CREATE FULLTEXT INDEX ON [dbo].[LineItems]
    ([CatalogNumber] LANGUAGE 1033, [Description] LANGUAGE 1033, [Url] LANGUAGE 1033, [Notes] LANGUAGE 1033, [CommodityId] LANGUAGE 1033, [ReceivedNotes] LANGUAGE 1033)
    KEY INDEX [PK_LineItems_1]
    ON [LineItems_DescriptionUrlNotesCatalognumberCommoditycode_SIDX];





