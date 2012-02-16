CREATE VIEW [dbo].[vRecentOrders]
	AS 

select row_number() over (order by orders.id) id
	, orders.id orderid, requestnumber, orders.datecreated
	, dateneeded, creator.firstname + ' ' + creator.lastname
from orders
	inner join users creator on creator.id = orders.createdby
	inner join ordertracking tracking on tracking.orderid = orders.id
where
	tracking.datecreated in ( select max(tracking.datecreated)
							  from ordertracking itracking
							  where tracking.orderid = itracking.orderid )