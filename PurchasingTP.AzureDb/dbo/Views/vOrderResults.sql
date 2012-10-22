CREATE VIEW [dbo].[vOrderResults]
AS
SELECT O.Id,
       O.DateCreated,
       O.DeliverTo,
       O.DeliverToEmail,
       O.Justification,
       U.FirstName + ' ' + U.LastName AS CreatedBy,
       O.RequestNumber
FROM   dbo.Orders AS O
       INNER JOIN
       dbo.Users AS U
       ON O.CreatedBy = U.Id;