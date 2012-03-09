CREATE FULLTEXT INDEX ON [dbo].[Orders]
    ([DeliverTo] LANGUAGE 1033, [DeliverToEmail] LANGUAGE 1033, [Justification] LANGUAGE 1033, [RequestNumber] LANGUAGE 1033)
    KEY INDEX [PK_Orders_1]
    ON [Orders_justification_SIDX];

