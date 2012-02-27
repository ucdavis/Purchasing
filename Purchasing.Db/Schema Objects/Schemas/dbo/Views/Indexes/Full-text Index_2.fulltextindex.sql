CREATE FULLTEXT INDEX ON [dbo].[vCustomFieldResults]
    ([Answer] LANGUAGE 1033)
    KEY INDEX [vCustomFieldResults_Id_UDX]
    ON [CustomFieldAnswers_Answer_SDX];

