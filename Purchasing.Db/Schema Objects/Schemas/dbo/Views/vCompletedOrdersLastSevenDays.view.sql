
CREATE VIEW [dbo].[vCompletedOrdersLastSevenDays]
AS
SELECT     dbo.Orders.Id, dbo.Orders.OrderTypeId, dbo.Orders.WorkgroupVendorId, dbo.Orders.WorkgroupAddressId, dbo.Orders.ShippingTypeId, dbo.Orders.DateNeeded, 
                      dbo.Orders.AllowBackorder, dbo.Orders.EstimatedTax, dbo.Orders.WorkgroupId, dbo.Orders.OrganizationId, dbo.Orders.PONumber, 
                      dbo.Orders.LastCompletedApprovalId, dbo.Orders.ShippingAmount, dbo.Orders.FreightAmount, dbo.Orders.DeliverTo, dbo.Orders.DeliverToEmail, 
                      dbo.Orders.Justification, dbo.Orders.OrderStatusCodeId, dbo.Orders.CreatedBy, dbo.Orders.DateCreated, dbo.Orders.HasAuthorizationNum, dbo.Orders.Total, 
                      dbo.Orders.CompletionReason, dbo.OrderStatusCodes.Id AS Expr1, dbo.OrderStatusCodes.Name, dbo.OrderStatusCodes.[Level], dbo.OrderStatusCodes.IsComplete, 
                      dbo.OrderStatusCodes.KfsStatus, dbo.OrderStatusCodes.ShowInFilterList, ot.OrderId, ot.otdatecreated, OrderTracking_1.Id AS Expr2, OrderTracking_1.Description, 
                      OrderTracking_1.OrderId AS Expr3, OrderTracking_1.DateCreated AS Expr4, OrderTracking_1.UserId, OrderTracking_1.OrderStatusCodeId AS Expr5
FROM         dbo.Orders INNER JOIN
                      dbo.OrderStatusCodes ON dbo.Orders.OrderStatusCodeId = dbo.OrderStatusCodes.Id INNER JOIN
                          (SELECT     OrderId, MAX(DateCreated) AS otdatecreated
                            FROM          dbo.OrderTracking
                            GROUP BY OrderId) AS ot ON dbo.Orders.Id = ot.OrderId INNER JOIN
                      dbo.OrderTracking AS OrderTracking_1 ON dbo.Orders.Id = OrderTracking_1.OrderId
WHERE     (dbo.OrderStatusCodes.IsComplete = 1) AND (ot.otdatecreated >= DATEADD(d, - 7, GETDATE()))