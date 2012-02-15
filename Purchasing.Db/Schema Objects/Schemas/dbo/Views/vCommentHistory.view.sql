CREATE VIEW dbo.vCommentHistory
AS
SELECT DISTINCT 
                      NEWID() AS id, dbo.OrderComments.OrderId, dbo.Orders.RequestNumber, dbo.Users.LastName + ', ' + dbo.Users.FirstName AS CreatedBy, 
                      dbo.OrderComments.Text AS comment, dbo.OrderComments.UserId AS createdbyuserid, dbo.OrderComments.DateCreated, access.UserId AS access
FROM         dbo.OrderComments INNER JOIN
                      dbo.Orders ON dbo.Orders.Id = dbo.OrderComments.OrderId INNER JOIN
                      dbo.Users ON dbo.Users.Id = dbo.OrderComments.UserId INNER JOIN
                          (SELECT DISTINCT OrderId, UserId
                            FROM          (SELECT     OrderId, UserId
                                                    FROM          dbo.OrderTracking
                                                    UNION
                                                    SELECT     dbo.Approvals.OrderId, dbo.Approvals.UserId
                                                    FROM         dbo.Approvals INNER JOIN
                                                                          dbo.OrderStatusCodes AS osc ON dbo.Approvals.OrderStatusCodeId = osc.Id INNER JOIN
                                                                          dbo.Orders AS Orders_2 ON dbo.Approvals.OrderId = Orders_2.Id INNER JOIN
                                                                          dbo.OrderStatusCodes AS osc2 ON Orders_2.OrderStatusCodeId = osc2.Id AND osc.[Level] = osc2.[Level]
                                                    WHERE     (dbo.Approvals.UserId IS NOT NULL)
                                                    UNION
                                                    SELECT     Approvals_1.OrderId, Approvals_1.SecondaryUserId
                                                    FROM         dbo.Approvals AS Approvals_1 INNER JOIN
                                                                          dbo.OrderStatusCodes AS osc ON Approvals_1.OrderStatusCodeId = osc.Id INNER JOIN
                                                                          dbo.Orders AS Orders_1 ON Approvals_1.OrderId = Orders_1.Id INNER JOIN
                                                                          dbo.OrderStatusCodes AS osc2 ON Orders_1.OrderStatusCodeId = osc2.Id AND osc.[Level] = osc2.[Level]
                                                    WHERE     (Approvals_1.SecondaryUserId IS NOT NULL)) AS blank) AS access ON access.OrderId = dbo.OrderComments.OrderId
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[31] 4[21] 2[30] 3) )"
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
         Begin Table = "OrderComments"
            Begin Extent = 
               Top = 6
               Left = 510
               Bottom = 125
               Right = 670
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "access"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 95
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
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
         Column = 1440
         Alias = 900
         Table = 1170
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
End', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vCommentHistory';






GO
/*EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'nd
   End
End', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vCommentHistory';*/


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vCommentHistory';



