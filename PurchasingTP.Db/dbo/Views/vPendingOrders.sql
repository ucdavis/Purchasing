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
		, lineitemsummary.summary
		, access.accessuserid
	from orders
		inner join users creator on creator.id = orders.createdby
		inner join ordertracking tracking on tracking.orderid = orders.id
		inner join orderstatuscodes codes on orders.orderstatuscodeid = codes.id
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
			) lineitemsummary on lineitemsummary.orderid = orders.id
		inner join vaccess access on access.OrderId = dbo.orders.id and editaccess = 1 and isadmin = 0
	where
		tracking.datecreated in ( select max(itracking.datecreated)
								  from ordertracking itracking
								  where tracking.orderid = itracking.orderid )
		and codes.IsComplete = 0
) Pending