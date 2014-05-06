

-- =============================================
-- Author:		Ken Taylor
-- Create date: February 12, 2014
-- Description:	Given a Kerberos/LoginId, Return a list of pending orders that the user has access to.
--	Notes:
--		Replaces vPendingOrders.
--		Query takes the recent orders that a person needs to access.
--
-- Usage:
/*
	select * from udf_GetPendingOrdersForLogin('bazemore') 
*/
-- Modifications:
-- =============================================
CREATE FUNCTION [dbo].[udf_GetPendingOrdersForLogin]
(
	-- Add the parameters for the function here
	@LoginId varchar(50) 
)
RETURNS 
@PendingOrders TABLE 
(
	   [id] int
      ,[orderid] int
      ,[requestnumber] varchar(20)
      ,[datecreated] datetime
      ,[dateneeded] datetime2(7)
      ,[creator] varchar(101)
      ,[lastactiondate] datetime2(7)
      ,[statusname] varchar(50)
      ,[summary] varchar(max)
      ,[accessuserid] varchar(10)
      ,[isDirectlyAssigned] bit
)
AS
BEGIN
	INSERT INTO @PendingOrders
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
			--inner join vOpenAccess access on access.OrderId = dbo.orders.id and isadmin = 0 and Edit = 1
			inner join udf_GetOpenOrdersForLogin(@LoginId) access on access.OrderId = dbo.orders.id and isadmin = 0 and Edit = 1
			left outer join approvals on approvals.OrderId = orders.id and (approvals.userid = access.accessuserid or approvals.SecondaryUserId = access.accessuserid) and approvals.orderstatuscodeid = orders.orderstatuscodeid
		where
			tracking.datecreated in ( select max(itracking.datecreated)
									  from ordertracking itracking
									  where tracking.orderid = itracking.orderid )
			and codes.IsComplete = 0
	) Pending
	
	RETURN 
END