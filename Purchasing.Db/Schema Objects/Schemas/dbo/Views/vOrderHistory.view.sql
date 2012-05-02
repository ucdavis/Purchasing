CREATE VIEW dbo.vOrderHistory
AS

select row_number() over (order by orderhistory.orderid) id
      ,orderhistory.[orderid]
      ,orderhistory.[RequestNumber]
      ,orderhistory.[workgroupid]
      ,orderhistory.[workgroupname]
      ,orderhistory.[Vendor]
      ,orderhistory.[CreatedBy]
      ,orderhistory.[DateCreated]
      ,orderhistory.[StatusId]
      ,orderhistory.[Status]
	  ,orderhistory.IsComplete
      ,orderhistory.[totalamount]
      ,orderhistory.[lineitems]
      ,orderhistory.[accountsubaccountsummary]
      ,orderhistory.[ShipTo]
      ,orderhistory.[AllowBackorder]
      ,orderhistory.[Restricted]
      ,orderhistory.[DateNeeded]
      ,orderhistory.[ShippingType]
      ,orderhistory.[ReferenceNumber]
      ,orderhistory.[LastActionDate]
      ,orderhistory.[LastActionUser]
      ,orderhistory.[Received]
      ,orderhistory.[ordertypeid]
      ,orderhistory.[ordertype]
	, vaccess.accessuserid, vAccess.readaccess, vAccess.editaccess, vAccess.isadmin, vAccess.isaway
from 
(
select o.id orderid,  o.RequestNumber
	, w.id workgroupid, w.name workgroupname
	, case 
		when o.WorkgroupVendorId is null then '--Unspecified--'
		when o.WorkgroupVendorId is not null and wv.line1 = 'N/A' and url is null then wv.name + '(No Adddress)' 
		when o.WorkgroupVendorId is not null and wv.line1 = 'N/A' and url is not null then wv.name + '('+wv.url+')' 
		else wv.name + ' (' + wv.Line1 + ', ' + wv.City + ', ' + wv.State + ', ' +wv.Zip +  ', ' + isnull(wv.CountryCode, '') + ')'
		end Vendor
	, creator.FirstName + ' ' + creator.LastName CreatedBy
	, o.DateCreated
	, osc.id StatusId, osc.name [Status], osc.IsComplete
	, totals.totalamount 
	, lineitemsummary.summary lineitems
	, accounts.accountsubaccountsummary
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
) orderhistory
	inner join vAccess on orderhistory.orderid = vAccess.orderid