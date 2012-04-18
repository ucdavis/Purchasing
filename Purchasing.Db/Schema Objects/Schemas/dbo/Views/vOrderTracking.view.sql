/*
	Summary:

	Query takes the order with each user's last tracking object and provides a line item summary
	of that order.  I believe this is used to show on the landing page.

	Date :	2/16/2012
	
*/

CREATE VIEW [dbo].[vOrderTracking]
	
	AS 

select distinct ROW_NUMBER() over (order by ordertracking.orderid) id, *
from
(
	select ordertracking.datecreated TrackingDate
		, ordertracking.orderid, orders.requestnumber
		, orders.datecreated, creator.firstname + ' ' + creator.lastname createdby
		, users.Id AccessUserId
		, lineitemsummary.summary
	from users
		 inner join ordertracking on ordertracking.userid = users.id
		 inner join orders on ordertracking.orderid = orders.id
		 inner join users creator on orders.createdby = creator.id
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
	where
		 ordertracking.datecreated in ( select max(tracking.datecreated)
									from ordertracking tracking
									where ordertracking.userid = tracking.userid)
) ordertracking