CREATE VIEW dbo.vOrderTrackingIndex
AS
SELECT OrderTracking.Id, OrderTracking.OrderId, dbo.OrderTracking.Description, OrderTracking.DateCreated AS ActionDate, dbo.OrderTracking.UserId, users.FirstName + ' ' + users.LastName AS userName, OrderTracking.OrderStatusCodeId AS TrackingStatusCode, 
             TrackingStatusCodes.IsComplete AS TrackingStatusComplete, Orders.WorkgroupId, workgroups.Name AS workgroupName, Orders.DateCreated AS OrderCreated, orders.OrderStatusCodeId AS CurrentStatusCodeId, OrderStatusCodes.Name AS CurrentStatusCode, 
             OrderStatusCodes.IsComplete
FROM   dbo.OrderTracking INNER JOIN
             dbo.Orders ON dbo.OrderTracking.OrderId = Orders.Id LEFT OUTER JOIN
             dbo.Users ON OrderTracking.UserId = Users.Id LEFT OUTER JOIN
             dbo.Workgroups ON orders.WorkgroupId = workgroups.Id INNER JOIN
             dbo.OrderStatusCodes ON orders.OrderStatusCodeId = OrderStatusCodes.Id INNER JOIN
             dbo.OrderStatusCodes AS TrackingStatusCodes ON OrderTracking.OrderStatusCodeId = TrackingStatusCodes.Id
GO
