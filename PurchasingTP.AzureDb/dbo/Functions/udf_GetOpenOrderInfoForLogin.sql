

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
CREATE FUNCTION [dbo].[udf_GetOpenOrderInfoForLogin]
(
	-- Add the parameters for the function here
	@LoginId varchar(50) 
)
RETURNS 
@OpenOrders TABLE 
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
	INSERT INTO @OpenOrders
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
	where CreatedBy = @LoginId AND ordertracking.datecreated in ( select max(itracking.datecreated)
									from ordertracking itracking
									where ordertracking.orderid = itracking.orderid )
	  and codes.iscomplete = 0									
	) OpenOrders

	RETURN 
END