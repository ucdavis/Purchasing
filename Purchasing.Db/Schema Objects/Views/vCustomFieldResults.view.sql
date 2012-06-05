
CREATE VIEW [dbo].[vCustomFieldResults] WITH SCHEMABINDING
AS
SELECT     CFA.Id, CFA.OrderId, O.RequestNumber, CF.Name AS Question, CFA.Answer
FROM         dbo.CustomFieldAnswers AS CFA INNER JOIN
                      dbo.CustomFields AS CF ON CFA.CustomFieldId = CF.Id INNER JOIN
                      dbo.Orders AS O ON CFA.OrderId = O.Id