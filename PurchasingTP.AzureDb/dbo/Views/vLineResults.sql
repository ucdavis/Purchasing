CREATE VIEW [dbo].[vLineResults]
AS
SELECT LI.Id,
       LI.OrderId,
       LI.Quantity,
       LI.Unit,
       O.RequestNumber,
       LI.CatalogNumber,
       LI.Description,
       LI.Url,
       LI.Notes,
       LI.CommodityId
FROM   dbo.LineItems AS LI
       INNER JOIN
       dbo.Orders AS O
       ON LI.OrderId = O.Id;