CREATE VIEW dbo.vOrderHistory
AS
select row_number() over (order by o.id) id, o.id orderid,  o.RequestNumber
	, w.id workgroupid, w.name workgroupname
	, case 
		when o.WorkgroupVendorId is null then '--Unspecified--'
		when o.WorkgroupVendorId is not null and wv.line1 = 'N/A' and url is null then wv.name + '(No Adddress)' 
		when o.WorkgroupVendorId is not null and wv.line1 = 'N/A' and url is not null then wv.name + '('+wv.url+')' 
		else wv.name + ' (' + wv.Line1 + ', ' + wv.City + ', ' + wv.State + ', ' +wv.Zip +  ', ' + isnull(wv.CountryCode, '') + ')'
		end Vendor
	, creator.FirstName + ' ' + creator.LastName CreatedBy
	, creator.id CreatorId
	, o.DateCreated
	, osc.id StatusId, osc.name [Status], osc.IsComplete
	, totals.totalamount 
	, lineitemsummary.summary lineitems
	, accounts.accountsubaccountsummary
	, cast(CASE WHEN isnull(charindex(',', accounts.accountsubaccountsummary), 0) <> 0 THEN 1 ELSE 0 END AS bit) HasAccountSplit
	, o.DeliverTo ShipTo
	, case when o.AllowBackorder = 1 then 'Yes'else 'No' end AllowBackorder
	, case when o.HasAuthorizationNum = 1 then 'Yes' else 'No' end Restricted
	, o.DateNeeded
	, st.name ShippingType
	, o.ReferenceNumber
	, lastaction.DateCreated LastActionDate
	, lastaction.lastuser LastActionUser
	, case when oreceived.received = 1 then 'Yes' else 'No' end Received
	, ot.id ordertypeid, ot.name ordertype
from orders o
	inner join workgroups w on o.WorkgroupId = w.id
	left outer join WorkgroupVendors wv on o.WorkgroupVendorId = wv.id
	inner join Users creator on o.CreatedBy = creator.id
	inner join OrderStatusCodes osc on o.OrderStatusCodeId = osc.id
	left outer join ordertypes ot on ot.id = o.OrderTypeId
	inner join (
		select orderid, sum(quantity * unitprice) totalamount from LineItems group by orderid
	) totals on o.id = totals.orderid
	inner join ( 
		select orderid, STUFF(
			(
				select ', ' + convert(varchar(10), a.quantity) + ' [' + a.Unit + '] ' + a.[description]
				from lineitems a
				where a.orderid = lineitems.orderid
				order by a.[description]
				for xml PATH('')
			), 1, 1, ''
		) as summary
		from lineitems
		group by orderid
	) lineitemsummary on lineitemsummary.orderid = o.id
	inner join (
		select orderid
			, STUFF(
				(
					select ', ' + ins.Account + isnull('[' + ins.subaccount + ']', '')
					from splits ins
					where os.OrderId = ins.OrderId
					order by ins.id
					for xml PATH('')
				), 1, 1, ''
			) as accountsubaccountsummary
		from splits os
		group by os.OrderId
	) accounts on accounts.OrderId = o.id
	left outer join ShippingTypes st on o.ShippingTypeId = st.id
	left outer join (
		select rorders.id orderid, case when ireceived.received is null then 1 else 0 end received
		from orders rorders
		left outer join (	
			select distinct LineItems.orderid liorderid, 0 received
			from LineItems
			where orderid = any ( select orderid from LineItems where Received = 0) 
		) ireceived on rorders.id = ireceived.liorderid
	) oreceived on o.id = oreceived.orderid
	inner join (
		select max(oot.id) otid, oot.orderid, oot.DateCreated, users.FirstName + ' ' + users.LastName lastuser
		from ordertracking oot
		inner join (select orderid, max(datecreated) maxdatecreated from ordertracking iot group by orderid) iot
				on iot.orderid = oot.OrderId and iot.maxdatecreated = oot.DateCreated
		inner join users on oot.userid = users.id
		group by oot.orderid, oot.DateCreated, users.FirstName, users.LastName
	) lastaction on lastaction.orderid = o.id


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[17] 4[16] 2[48] 3) )"
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
      Begin ColumnWidths = 11
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
End', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vOrderHistory';




GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vOrderHistory';

