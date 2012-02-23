CREATE FULLTEXT INDEX ON [dbo].[Orders]
    ([Justification] LANGUAGE 1033)
    KEY INDEX [PK_Orders_1]
    ON [Orders_justification_SIDX];

