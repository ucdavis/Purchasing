CREATE VIEW [dbo].[vOpenOrdersByUser]
	AS 

select row_number() over (order by orderid) id, *
from (
	select distinct orders.id orderid, requestnumber, orders.datecreated, dateneeded
		, creator.firstname + ' ' + creator.lastname creator
		, ordertracking.datecreated lastactiondate
		, codes.name statusname
		, orders.LineItemSummary as summary
		, createdby accessuserid
		, wv.name VendorName
	from orders
		inner join users creator on creator.id = orders.createdby
		inner join ordertracking on orders.id = ordertracking.orderid
		inner join orderstatuscodes codes on orders.orderstatuscodeid = codes.id		
		left outer join WorkgroupVendors wv on orders.WorkgroupVendorId = wv.id
	where ordertracking.datecreated in ( select max(itracking.datecreated)
									from ordertracking itracking
									where ordertracking.orderid = itracking.orderid )
	  and codes.iscomplete = 0									
) OpenOrders