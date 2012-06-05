CREATE VIEW dbo.vCompletedOrdersThisMonth
AS
SELECT     dbo.Orders.Id, dbo.Orders.OrderStatusCodeId, dbo.OrderStatusCodes.Id AS OrderStatusCodesId, dbo.OrderStatusCodes.[Level], dbo.OrderStatusCodes.IsComplete, 
                      dbo.OrderStatusCodes.KfsStatus, ot.OrderId, ot.otdatecreated, OrderTracking_1.Id AS OrderTrackingId, OrderTracking_1.Description, 
                      OrderTracking_1.OrderId AS OrderTrackingOrderId, OrderTracking_1.DateCreated AS OrderTrackingDateCreated, OrderTracking_1.UserId, 
                      OrderTracking_1.OrderStatusCodeId AS OrderTrackingOrderStatusCodeId
FROM         dbo.Orders INNER JOIN
                      dbo.OrderStatusCodes ON dbo.Orders.OrderStatusCodeId = dbo.OrderStatusCodes.Id INNER JOIN
                          (SELECT     OrderId, MAX(DateCreated) AS otdatecreated
                            FROM          dbo.OrderTracking
                            GROUP BY OrderId) AS ot ON dbo.Orders.Id = ot.OrderId INNER JOIN
                      dbo.OrderTracking AS OrderTracking_1 ON dbo.Orders.Id = OrderTracking_1.OrderId
WHERE     (dbo.OrderStatusCodes.IsComplete = 1) AND (DATEPART(Month, ot.otdatecreated) = DATEPART(Month, GETDATE()))
GO



GO



GO


