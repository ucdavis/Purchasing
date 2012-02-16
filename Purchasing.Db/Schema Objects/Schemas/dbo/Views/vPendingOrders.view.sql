﻿/*
	Summary:

	Query takes the recent orders that a person needs to access.

	Date :	2/16/2012
	
*/

CREATE VIEW [dbo].[vPendingOrders]
	AS 

select row_number() over (order by orders.id) id
	, orders.id orderid, requestnumber, orders.datecreated
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
		SELECT DISTINCT OrderId, UserId	
		FROM (
		SELECT     OrderId, UserId
		FROM          dbo.OrderTracking
		UNION
		SELECT     dbo.Approvals.OrderId, dbo.Approvals.UserId
		FROM         dbo.Approvals INNER JOIN
							  dbo.OrderStatusCodes AS osc ON dbo.Approvals.OrderStatusCodeId = osc.Id INNER JOIN
							  dbo.Orders AS Orders_2 ON dbo.Approvals.OrderId = Orders_2.Id INNER JOIN
							  dbo.OrderStatusCodes AS osc2 ON Orders_2.OrderStatusCodeId = osc2.Id AND osc.[Level] = osc2.[Level]
		WHERE     (dbo.Approvals.UserId IS NOT NULL)
		UNION
		SELECT     Approvals_1.OrderId, Approvals_1.SecondaryUserId
		FROM         dbo.Approvals AS Approvals_1 INNER JOIN
							  dbo.OrderStatusCodes AS osc ON Approvals_1.OrderStatusCodeId = osc.Id INNER JOIN
							  dbo.Orders AS Orders_1 ON Approvals_1.OrderId = Orders_1.Id INNER JOIN
							  dbo.OrderStatusCodes AS osc2 ON Orders_1.OrderStatusCodeId = osc2.Id AND osc.[Level] = osc2.[Level]
		WHERE     (Approvals_1.SecondaryUserId IS NOT NULL)) AS blank
	) AS access ON access.OrderId = dbo.orders.id
where
	tracking.datecreated in ( select max(itracking.datecreated)
							  from ordertracking itracking
							  where tracking.orderid = itracking.orderid )