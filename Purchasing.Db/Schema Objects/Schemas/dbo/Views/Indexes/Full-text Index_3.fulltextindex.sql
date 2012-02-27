CREATE FULLTEXT INDEX ON [dbo].[vCommentResults]
    ([Text] LANGUAGE 1033)
    KEY INDEX [vCommentResults_Id_UDX]
    ON [OrderComments_Answer_SDX];

