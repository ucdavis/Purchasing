CREATE FULLTEXT INDEX ON [dbo].[vOrderResults]
    ([DeliverTo] LANGUAGE 1033, [DeliverToEmail] LANGUAGE 1033, [Justification] LANGUAGE 1033, [RequestNumber] LANGUAGE 1033)
    KEY INDEX [vOrderResults_Id_UDX]
    ON [Orders_justification_SIDX];

