﻿CREATE VIEW dbo.vOrderHistory
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
	, accounts.accountsubaccountsummary AccountSummary
	, cast(CASE WHEN isnull(charindex(',', accounts.accountsubaccountsummary), 0) <> 0 THEN 1 ELSE 0 END AS bit) HasAccountSplit
	, o.DeliverTo ShipTo
	, o.DeliverToEmail ShipToEmail
	, o.Justification
	, case when o.AllowBackorder = 1 then 'Yes'else 'No' end AllowBackorder
	, case when o.HasAuthorizationNum = 1 then 'Yes' else 'No' end Restricted
	, o.DateNeeded
	, st.name ShippingType
	, o.ReferenceNumber
	, o.PoNumber
	, o.ShippingAmount
	, lastaction.DateCreated LastActionDate
	, lastaction.lastuser LastActionUser
	, case when oreceived.received = 1 then 'Yes' else 'No' end Received
	, ot.id ordertypeid, ot.name ordertype
	, approvers.approver, AccountManagers.AccountManager, Purchasers.Purchaser
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
	inner join (
		select oaps.id orderid
			, STUFF (
				(
					select ', ' + 
						(case when aps.userid is not null and aps.SecondaryUserId is null then u.firstname + ' ' + u.lastname 
							  when aps.userid is not null and aps.SecondaryUserId is not null then u.firstname + ' ' + u.lastname + ', ' + su.firstname + ' ' + su.lastname
							  when aps.userid is null then '[Workgroup Approver]'
						end)
					from (select distinct orderid, userid, secondaryuserid, orderstatuscodeid from approvals where OrderStatusCodeId in ('AP', 'CA')) aps
						left outer join users u on aps.userid = u.id
						left outer join users su on aps.SecondaryUserId = su.id
					where aps.orderid = oaps.id
					for xml PATH('')
				), 1, 1, '') as approver
		from orders oaps
	) approvers on approvers.orderid = o.id
	inner join (
		select oaps.id orderid
			, STUFF (
				(
					select ', ' + 
						(case when aps.userid is not null and aps.SecondaryUserId is null then u.firstname + ' ' + u.lastname 
							  when aps.userid is not null and aps.SecondaryUserId is not null then u.firstname + ' ' + u.lastname + ', ' + su.firstname + ' ' + su.lastname
							  when aps.userid is null then '[Workgroup Account Managers]'
						end)
					from (select distinct orderid, userid, secondaryuserid, orderstatuscodeid from approvals where OrderStatusCodeId = 'AM') aps
						left outer join users u on aps.userid = u.id
						left outer join users su on aps.SecondaryUserId = su.id
					where aps.orderid = oaps.id
					for xml PATH('')
				), 1, 1, '') as AccountManager
		from orders oaps
	) AccountManagers on AccountManagers.orderid = o.id
	inner join (
		select oaps.id orderid
			, STUFF (
				(
					select ', ' + 
						(case when aps.userid is not null and aps.SecondaryUserId is null then u.firstname + ' ' + u.lastname 
							  when aps.userid is not null and aps.SecondaryUserId is not null then u.firstname + ' ' + u.lastname + ', ' + su.firstname + ' ' + su.lastname
							  when aps.userid is null then '[Workgroup Purchasers]'
						end)
					from (select distinct orderid, userid, secondaryuserid, orderstatuscodeid from approvals where OrderStatusCodeId = 'PR') aps
						left outer join users u on aps.userid = u.id
						left outer join users su on aps.SecondaryUserId = su.id
					where aps.orderid = oaps.id
					for xml PATH('')
				), 1, 1, '') as Purchaser
		from orders oaps
	) Purchasers on Purchasers.orderid = o.id
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