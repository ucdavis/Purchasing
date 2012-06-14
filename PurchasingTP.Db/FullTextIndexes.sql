CREATE FULLTEXT INDEX ON [dbo].[CustomFieldAnswers]
    ([Answer] LANGUAGE 1033)
    KEY INDEX [PK_CustomFieldAnswers]
    ON [CustomFieldAnswers_Answer_SDX];


GO
CREATE FULLTEXT INDEX ON [dbo].[LineItems]
    ([CatalogNumber] LANGUAGE 1033, [Description] LANGUAGE 1033, [Url] LANGUAGE 1033, [Notes] LANGUAGE 1033, [CommodityId] LANGUAGE 1033, [ReceivedNotes] LANGUAGE 1033)
    KEY INDEX [PK_LineItems_1]
    ON [LineItems_DescriptionUrlNotesCatalognumberCommoditycode_SIDX];


GO
CREATE FULLTEXT INDEX ON [dbo].[OrderComments]
    ([Text] LANGUAGE 1033)
    KEY INDEX [PK_OrderComments]
    ON [OrderComments_Answer_SDX];


GO


GO
CREATE FULLTEXT INDEX ON [dbo].[Orders]
    ([DeliverTo] LANGUAGE 1033, [DeliverToEmail] LANGUAGE 1033, [Justification] LANGUAGE 1033, [RequestNumber] LANGUAGE 1033)
    KEY INDEX [PK_Orders_1]
    ON [Orders_justification_SIDX];


GO


