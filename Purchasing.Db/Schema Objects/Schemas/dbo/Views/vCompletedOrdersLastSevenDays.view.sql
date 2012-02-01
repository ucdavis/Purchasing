CREATE VIEW dbo.vCompletedOrdersLastSevenDays
AS
SELECT     dbo.Orders.Id, dbo.Orders.OrderTypeId, dbo.Orders.WorkgroupVendorId, dbo.Orders.WorkgroupAddressId, dbo.Orders.ShippingTypeId, dbo.Orders.DateNeeded, 
                      dbo.Orders.AllowBackorder, dbo.Orders.EstimatedTax, dbo.Orders.WorkgroupId, dbo.Orders.OrganizationId, dbo.Orders.PONumber, 
                      dbo.Orders.LastCompletedApprovalId, dbo.Orders.ShippingAmount, dbo.Orders.FreightAmount, dbo.Orders.DeliverTo, dbo.Orders.DeliverToEmail, 
                      dbo.Orders.Justification, dbo.Orders.OrderStatusCodeId, dbo.Orders.CreatedBy, dbo.Orders.DateCreated, dbo.Orders.HasAuthorizationNum, dbo.Orders.Total, 
                      dbo.Orders.CompletionReason, dbo.OrderStatusCodes.Id AS OrderStatusCodesId, dbo.OrderStatusCodes.Name, dbo.OrderStatusCodes.[Level], 
                      dbo.OrderStatusCodes.IsComplete, dbo.OrderStatusCodes.KfsStatus, dbo.OrderStatusCodes.ShowInFilterList, ot.OrderId, ot.otdatecreated, 
                      OrderTracking_1.Id AS OrderTrackingId, OrderTracking_1.Description, OrderTracking_1.OrderId AS OrderTrackingOrderId, 
                      OrderTracking_1.DateCreated AS OrderTrackingDateCreated, OrderTracking_1.UserId, OrderTracking_1.OrderStatusCodeId AS OrderTrackingOrderStatusCodeId
FROM         dbo.Orders INNER JOIN
                      dbo.OrderStatusCodes ON dbo.Orders.OrderStatusCodeId = dbo.OrderStatusCodes.Id INNER JOIN
                          (SELECT     OrderId, MAX(DateCreated) AS otdatecreated
                            FROM          dbo.OrderTracking
                            GROUP BY OrderId) AS ot ON dbo.Orders.Id = ot.OrderId INNER JOIN
                      dbo.OrderTracking AS OrderTracking_1 ON dbo.Orders.Id = OrderTracking_1.OrderId
WHERE     (dbo.OrderStatusCodes.IsComplete = 1) AND (ot.otdatecreated >= DATEADD(d, - 7, GETDATE()))
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vCompletedOrdersLastSevenDays';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'dth = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1980
         Alias = 2655
         Table = 2355
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vCompletedOrdersLastSevenDays';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[15] 4[46] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Orders"
            Begin Extent = 
               Top = 193
               Left = 99
               Bottom = 312
               Right = 312
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OrderStatusCodes"
            Begin Extent = 
               Top = 45
               Left = 423
               Bottom = 164
               Right = 588
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ot"
            Begin Extent = 
               Top = 49
               Left = 32
               Bottom = 138
               Right = 192
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OrderTracking_1"
            Begin Extent = 
               Top = 180
               Left = 514
               Bottom = 299
               Right = 697
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 38
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Wi', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vCompletedOrdersLastSevenDays';

