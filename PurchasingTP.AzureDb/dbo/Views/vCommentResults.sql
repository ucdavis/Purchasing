CREATE VIEW [dbo].[vCommentResults]
AS
SELECT OC.Id,
       OC.OrderId,
       O.RequestNumber,
       OC.DateCreated,
       OC.Text,
       U.FirstName + ' ' + U.LastName AS CreatedBy
FROM   dbo.OrderComments AS OC
       INNER JOIN
       dbo.Orders AS O
       ON OC.OrderId = O.Id
       INNER JOIN
       dbo.Users AS U
       ON OC.UserId = U.Id;