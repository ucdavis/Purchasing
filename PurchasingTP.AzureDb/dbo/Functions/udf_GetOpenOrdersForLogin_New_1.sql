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

	select * from udf_GetOpenOrdersForLogin('rajahn')
*/
-- Modifications:
--	2014-02-10 by kjt: Revised logic to reduce number of executions, i.e. loop iterations.
--	2014-02-12 by kjt: Revised logic once again to maximize efficiency.
--	2017-02-28 by kjt: Revised logic to improve performance.
-- =============================================
CREATE FUNCTION [dbo].[udf_GetOpenOrdersForLogin_New]
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
		SELECT * FROM (
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
                and osc.iscomplete = 0
                and osc.id not in ('CN', 'CP', 'OC', 'OD')
            
			left outer join approvals ap on o.id = ap.orderid
                and ap.Completed = 0
                and (
                    ap.userid in ( select userid from workgrouppermissions where workgroupid = o.workgroupid ) 
                    or 
                    ap.userid is null
				)
			inner join orderstatuscodes aposc on ap.OrderStatusCodeId = aposc.id
                and aposc.level = osc.Level
			left outer join workgrouppermissions wp on o.workgroupid = wp.workgroupid and ap.orderstatuscodeid = wp.roleid
                and (
				wp.isadmin = 0
				or
					(
					wp.isadmin = 1 and wp.isfullfeatured = 1
					)
				)												
			left outer join users ouser on ouser.id = ap.userid
		) t1
		WHERE accessuserid = @LoginId
		-- Moved remaining portion of where clause segments to individual joins.
			
		union

		-- ad hoc permissions
		select distinct o.id orderid
			, ap.userid accessuserid
			, 0 isadmin
			, ap.orderstatuscodeid accesslevel
			, 1 Edit, 1 [Read]
		from orders o
			inner join orderstatuscodes osc on o.orderstatuscodeid = osc.id
				and osc.iscomplete = 0
				and osc.id not in ('CN', 'CP', 'OC', 'OD')
			left outer join approvals ap on o.id = ap.orderid
				and ap.Completed = 0
				and ap.userid not in ( select userid from workgrouppermissions where workgroupid = o.workgroupid )
			inner join orderstatuscodes aposc on ap.OrderStatusCodeId = aposc.id
				and aposc.level = osc.level
		where 
		  ap.userid = @LoginId
		  -- Moved remaining portion of where clause segments to individual joins.
		
		union

		-- override, provides admin permissions
		select o.id orderid, userid, isadmin, wp.roleid
			, 1 Edit, 1 [Read]
		from orders o
			inner join workgrouppermissions wp on o.workgroupid = wp.workgroupid 
			and wp.roleId not in ('CN', 'CP', 'OC', 'OD')
			and o.OrderStatusCodeId = wp.roleid
			and wp.isadmin = 1 and wp.IsFullFeatured = 0
		where 
			userid = @LoginId
			-- Moved remaining portion of where clause segments to individual joins.
			
		union
	
		-- secondary Conditional Approval
		select ap.OrderId, ap.SecondaryUserId accessuserid, cast(0 as bit) isadmin, ap.OrderStatusCodeId
			, 1 Edit, 1 [Read]
		from approvals ap
			inner join orders o on ap.OrderId = o.id
			inner join OrderStatusCodes oosc on o.orderstatuscodeid = oosc.id
				and oosc.id not in ('CN', 'CP', 'OC', 'OD')
			inner join OrderStatusCodes aposc on ap.OrderStatusCodeId = aposc.id 
				and aposc.level = oosc.level
		where 
			ap.SecondaryUserId = @LoginId
			and ap.OrderStatusCodeId = 'CA'
			and ap.Completed = 0
			and ap.SecondaryUserId is not null
			-- Moved remaining portion of where clause segments to individual joins.
			
		union
		
		-- Primary Conditional Approval 
		select ap.OrderId, ap.UserId accessuserid, cast(0 as bit) isadmin, ap.OrderStatusCodeId
			, 1 Edit, 1 [Read]
		from approvals ap
			inner join orders o on ap.OrderId = o.id
			inner join OrderStatusCodes aposc on ap.OrderStatusCodeId = aposc.id 
				and aposc.id not in ('CN', 'CP', 'OC', 'OD')
			inner join OrderStatusCodes oosc on o.orderstatuscodeid = oosc.id
				and aposc.level = oosc.level			
		where 
			ap.UserId = @LoginId
			and ap.OrderStatusCodeId = 'CA'
			and ap.Completed = 0
			-- Moved remaining portion of where clause segments to individual joins.
			
		union
		
		select orderid, userid accessuserid, 0 [admin], ordertracking.OrderStatusCodeId
			, 0 Edit, 1 [Read]
		from ordertracking
			inner join orders o on ordertracking.orderid = o.id 
				and o.orderstatuscodeid not in ('CN', 'CP', 'OC', 'OD')
			-- This was a redundant join, 
			--inner join OrderStatusCodes osc on o.orderstatuscodeid = osc.id
		where 
			userid = @LoginId
			-- Moved remaining portion of where clause segments to individual joins.
			
		union

		-- reviewer role
		select o.id orderid, wp.userid accessuserid
			, cast (case when wp.isadmin = 1 and wp.isfullfeatured = 0 then 1 else 0 end as bit) [admin]
			, wp.RoleId orderstatuscodeid
			, 0 Edit, 1 [Read]
		from workgrouppermissions wp
			inner join orders o on o.WorkgroupId = wp.WorkgroupId
			inner join workgroups w on wp.workgroupid = w.id and w.IsActive = 1
				and wp.roleid = 'RV'
			inner join OrderStatusCodes osc on o.orderstatuscodeid = osc.id
				and osc.id not in ('CN', 'CP', 'OC', 'OD')
		where 
		  wp.userid = @LoginId
		  -- Moved remaining portion of where clause segments to individual joins.

	) vopenaccess
	where accessuserid is not null
	
	RETURN 
END