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
	, access.UserId as accessuserid
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
		inner join (
			-- primary user specified
			select orders.id orderid, approvals.userid
			from orders
				-- order's current status
				inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
				-- approvals at the same level as the order status and not completed
				inner join approvals on approvals.orderid = orders.id and approvals.orderstatuscodeid = os.id and approvals.completed = 0
			where approvals.userid is not null
			union
			-- secondary user specified
			select orders.id orderid, approvals.secondaryuserid
			from orders
				-- order's current status
				inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
				-- approvals at the same level as the order status and not completed
				inner join approvals on approvals.orderid = orders.id and approvals.orderstatuscodeid = os.id and approvals.completed = 0
			where secondaryuserid is not null
			union
			-- workgroup permissions
			select orders.id orderid, workgrouppermissions.userid
			from orders
				-- order's current status
				inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
				-- approvals at the same level as the order status and not completed
				inner join approvals on approvals.orderid = orders.id and approvals.orderstatuscodeid = os.id and approvals.completed = 0
				-- join with the workgroup
				inner join workgroups on orders.workgroupid = workgroups.id
				-- workgroup permissions
				inner join workgrouppermissions on workgroups.id = workgrouppermissions.workgroupid and orders.orderstatuscodeid = workgrouppermissions.roleid
			where approvals.userid is null and approvals.secondaryuserid is null
		) AS access ON access.OrderId = dbo.orders.id
	where
		tracking.datecreated in ( select max(itracking.datecreated)
								  from ordertracking itracking
								  where tracking.orderid = itracking.orderid )
		and codes.IsComplete = 0
) Pending							  							  
