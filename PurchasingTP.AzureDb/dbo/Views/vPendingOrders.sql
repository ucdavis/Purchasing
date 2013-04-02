/*
	Summary:

	Query takes the recent orders that a person needs to access.

	Date :	2/16/2012
	
*/

CREATE VIEW [dbo].[vPendingOrders]
	AS 

select row_number() over (order by pending.orderid) id, *
from 
(
		select distinct orders.id orderid, requestnumber, orders.datecreated
		, dateneeded, creator.firstname + ' ' + creator.lastname creator
		, tracking.datecreated lastactiondate
		, codes.name statusname
		, orders.LineItemSummary as summary
		, access.accessuserid
		, cast(case when approvals.id is not null then 1 else 0 end as bit) isDirectlyAssigned
	from orders
		inner join users creator on creator.id = orders.createdby
		inner join ordertracking tracking on tracking.orderid = orders.id
		inner join orderstatuscodes codes on orders.orderstatuscodeid = codes.id		
		inner join vOpenAccess access on access.OrderId = dbo.orders.id and isadmin = 0 and Edit = 1
		left outer join approvals on approvals.OrderId = orders.id and (approvals.userid = access.accessuserid or approvals.SecondaryUserId = access.accessuserid) and approvals.orderstatuscodeid = orders.orderstatuscodeid
	where
		tracking.datecreated in ( select max(itracking.datecreated)
								  from ordertracking itracking
								  where tracking.orderid = itracking.orderid )
		and codes.IsComplete = 0
) Pending