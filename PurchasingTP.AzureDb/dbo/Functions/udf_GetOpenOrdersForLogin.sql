

-- =============================================
-- Author:		Ken Taylor
-- Create date: February 5, 2014
-- Description:	Given a Kerberos/LoginId, Return a list of open orders that the user has access to.
--	Notes:
--		Replaces vOpenAccess.
--		Permissions access for open orders only.  Orders that are not in the following status codes: ('CN', 'CP', 'OC', 'OD')
--
-- Usage:
/*
	select * from udf_GetOpenOrdersForLogin('bazemore') 
*/
-- Modifications:
--	2014-02-10 by kjt: Revised logic to reduce number of executions, i.e. loop iterations.
--	2014-02-12 by kjt: Revised logic once again to maximize efficiency.
-- =============================================
CREATE FUNCTION [dbo].[udf_GetOpenOrdersForLogin]
(
	-- Add the parameters for the function here
	@LoginId varchar(50) 
)
RETURNS 
@OpenAccessOrders TABLE 
(
	id int, 
	orderid int,
	accessuserid varchar(10),
	isadmin bit,
	accesslevel char(2),
	Edit bit,
	[Read] bit
)
AS
BEGIN
	INSERT INTO @OpenAccessOrders
	select ROW_NUMBER() over (order by orderid) id, *
	from (
		-- regular permissions
		select distinct o.id orderid
		, case when ap.userid is null then wp.userid
			when ap.userid is not null and ouser.isaway = 1 then wp.userid
			else ap.userid
			end accessuserid
		, 0 isadmin
		, ap.orderstatuscodeid accesslevel
		, 1 Edit, 1 [Read]
		from orders o
			inner join orderstatuscodes osc on o.orderstatuscodeid = osc.id
			left outer join approvals ap on o.id = ap.orderid
			inner join orderstatuscodes aposc on ap.OrderStatusCodeId = aposc.id
			left outer join workgrouppermissions wp on o.workgroupid = wp.workgroupid and ap.orderstatuscodeid = wp.roleid												
			left outer join users ouser on ouser.id = ap.userid
		where 
			case when ap.userid is null then wp.userid
			when ap.userid is not null and ouser.isaway = 1 then wp.userid
			else ap.userid
			end = @LoginId
			and ap.Completed = 0
			and osc.iscomplete = 0
			and aposc.level = osc.Level
			and (
				wp.isadmin = 0
				or
					(
					wp.isadmin = 1 and wp.isfullfeatured = 1
					)
				)
			and (
				ap.userid in ( select userid from workgrouppermissions where workgroupid = o.workgroupid ) 
				or 
				ap.userid is null
				)
			and o.orderstatuscodeid not in ('CN', 'CP', 'OC', 'OD')
			--and osc.id not in ('CN', 'CP', 'OC', 'OD')
			
		union

		-- ad hoc permissons
		select distinct o.id orderid
			, ap.userid accessuserid
			, 0 isadmin
			, ap.orderstatuscodeid accesslevel
			, 1 Edit, 1 [Read]
		from orders o
			inner join orderstatuscodes osc on o.orderstatuscodeid = osc.id
			left outer join approvals ap on o.id = ap.orderid
			inner join orderstatuscodes aposc on ap.OrderStatusCodeId = aposc.id
		where 
		  ap.userid = @LoginId
		  and ap.Completed = 0
		  and osc.iscomplete = 0
		  and aposc.level = osc.level
		  and ap.userid not in ( select userid from workgrouppermissions where workgroupid = o.workgroupid )
		--and o.orderstatuscodeid not in ('CN', 'CP', 'OC', 'OD')
		  and osc.id not in ('CN', 'CP', 'OC', 'OD')
		
		union

		-- override, provides admin permissions
		select o.id orderid, userid, isadmin, wp.roleid
			, 1 Edit, 1 [Read]
		from orders o
			inner join workgrouppermissions wp on o.workgroupid = wp.workgroupid and o.OrderStatusCodeId = wp.roleid
			
		where 
			userid = @LoginId
			and wp.isadmin = 1 and wp.IsFullFeatured = 0
			and o.orderstatuscodeid not in ('CN', 'CP', 'OC', 'OD')
			
		union
	
		-- secondary Conditional Approval
		select ap.OrderId, ap.SecondaryUserId accessuserid, cast(0 as bit) isadmin, ap.OrderStatusCodeId
			, 1 Edit, 1 [Read]
		from approvals ap
			inner join orders o on ap.OrderId = o.id
			inner join OrderStatusCodes aposc on ap.OrderStatusCodeId = aposc.id
			inner join OrderStatusCodes oosc on o.orderstatuscodeid = oosc.id
		where 
			ap.SecondaryUserId = @LoginId
			and ap.OrderStatusCodeId = 'CA'
			and ap.SecondaryUserId is not null
			and aposc.level = oosc.level
			and ap.Completed = 0
			and o.orderstatuscodeid not in ('CN', 'CP', 'OC', 'OD')
			--and oosc.id not in ('CN', 'CP', 'OC', 'OD')
			
		union
		
		-- Primary Conditional Approval 
		select ap.OrderId, ap.UserId accessuserid, cast(0 as bit) isadmin, ap.OrderStatusCodeId
			, 1 Edit, 1 [Read]
		from approvals ap
			inner join orders o on ap.OrderId = o.id
			inner join OrderStatusCodes aposc on ap.OrderStatusCodeId = aposc.id
			inner join OrderStatusCodes oosc on o.orderstatuscodeid = oosc.id
		where 
			ap.UserId = @LoginId
			and ap.OrderStatusCodeId = 'CA'
			and aposc.level = oosc.level
			and ap.Completed = 0
			and o.orderstatuscodeid not in ('CN', 'CP', 'OC', 'OD')
			--and oosc.id not in ('CN', 'CP', 'OC', 'OD')

		union
		
		select orderid, userid accessuserid, 0 [admin], ordertracking.OrderStatusCodeId
			, 0 Edit, 1 [Read]
		from ordertracking
			inner join orders o on ordertracking.orderid = o.id
			--inner join OrderStatusCodes osc on o.orderstatuscodeid = osc.id
		where 
			userid = @LoginId
			and o.orderstatuscodeid not in ('CN', 'CP', 'OC', 'OD')
			--and osc.id not in ('CN', 'CP', 'OC', 'OD')

		union

		-- reviewer role
		select o.id orderid, wp.userid accessuserid
			, cast (case when wp.isadmin = 1 and wp.isfullfeatured = 0 then 1 else 0 end as bit) [admin]
			, wp.RoleId orderstatuscodeid
			, 0 Edit, 1 [Read]
		from workgrouppermissions wp
			inner join orders o on o.WorkgroupId = wp.WorkgroupId
			inner join workgroups w on wp.workgroupid = w.id and w.IsActive = 1
			inner join OrderStatusCodes osc on o.orderstatuscodeid = osc.id
		where 
		  wp.userid = @LoginId
		  and wp.roleid = 'RV'
		  --and o.orderstatuscodeid not in ('CN', 'CP', 'OC', 'OD')
		  and osc.id not in ('CN', 'CP', 'OC', 'OD')

		) vopenaccess
	where accessuserid is not null
	
	RETURN 
END