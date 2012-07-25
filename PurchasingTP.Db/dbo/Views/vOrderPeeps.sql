CREATE VIEW dbo.vOrderPeeps
AS
select ROW_NUMBER() over (order by orderid) id, orderid, workgroupid, orderstatuscodeid, userid, fullname, administrative, sharedorcluster, roleid
from (
	select orders.id orderid, orders.workgroupid, approvals.OrderStatusCodeId, wp.userid, users.firstname + ' ' + users.lastname fullname, 0 administrative, 0 sharedorcluster, wp.roleid
	from orders 
		inner join approvals on approvals.OrderId = orders.id
		inner join WorkgroupPermissions wp on orders.WorkgroupId = wp.workgroupid and approvals.OrderStatusCodeId = wp.roleid
		inner join Workgroups on wp.WorkgroupId = workgroups.id
		inner join users on wp.userid = users.id
	where approvals.UserId is null
	  and workgroups.IsActive = 1
	union
	select orders.id orderid, orders.WorkgroupId, approvals.OrderStatusCodeId, admins.userid, admins.fullname, 1 administrative, admins.IsFullFeatured, admins.roleid
	from orders
		inner join approvals on approvals.OrderId = orders.id
		inner join (
			select workgroups.id adminworkgroupid, workgroups.IsFullFeatured, descendants.rollupparentid, descendants.orgid, descendantwrkgrp.id descendantworkgroupid
				, users.id userid, users.firstname + ' ' + users.lastname fullname
				, perms.roleid
			from workgroups
				inner join WorkgroupsXOrganizations on workgroups.id = WorkgroupsXOrganizations.workgroupid
				inner join vorganizationdescendants descendants on workgroupsxorganizations.organizationid = descendants.rollupparentid
				inner join workgroupsxorganizations descendantorgs on descendantorgs.organizationid = descendants.orgid
				inner join workgroups descendantwrkgrp on descendantwrkgrp.id = descendantorgs.workgroupid and descendantwrkgrp.administrative = 0
				inner join workgrouppermissions perms on workgroups.id = perms.workgroupid
				inner join users on perms.userid = users.id
			where workgroups.administrative = 1
			  and workgroups.IsActive = 1
		) admins on orders.workgroupid = admins.descendantworkgroupid
) peeps
where orderstatuscodeid = roleid

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
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
End', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vOrderPeeps';




GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vOrderPeeps';

