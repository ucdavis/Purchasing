--		*CA APPROVALS
--		*Specified as primary user
--		*Specified as secondary user
--
-- Usage:
/*
	select * from udf_GetEditAccessOrdersForLogin('bazemore')
*/
-- =============================================
CREATE FUNCTION udf_GetEditAccessRecentOrdersForLogin
(
	-- Add the parameters for the function here
	@LoginId varchar(50),
	@Cutoff datetime
)
RETURNS
@EditAccessOrders TABLE
(
	id int,
	orderid int,
	accessuserid varchar(10),
	isadmin bit,
	accesslevel char(2)
)
AS
BEGIN
	INSERT INTO @EditAccessOrders
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
    	    and o.DateLastAction > @Cutoff
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

		union

		-- ad hoc permissons
		select distinct o.id orderid
			, ap.userid accessuserid
			, 0 isadmin
			, ap.orderstatuscodeid accesslevel
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
		  and o.DateLastAction > @Cutoff

		union

		-- override, provides admin permissions
		select o.id orderid, userid, isadmin, wp.roleid
		from orders o
			inner join workgrouppermissions wp on o.workgroupid = wp.workgroupid and o.OrderStatusCodeId = wp.roleid
		where
			wp.userid = @LoginId
			and wp.isadmin = 1
		    and wp.IsFullFeatured = 0
		    and o.DateLastAction > @Cutoff

		union

		-- secondary Conditional Approval
		select ap.OrderId, ap.SecondaryUserId accessuserid, cast(0 as bit) isadmin, ap.OrderStatusCodeId
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
    		and o.DateLastAction > @Cutoff

		union

		-- Primary Conditional Approval
		select ap.OrderId, ap.UserId accessuserid, cast(0 as bit) isadmin, ap.OrderStatusCodeId
		from approvals ap
			inner join orders o on ap.OrderId = o.id
			inner join OrderStatusCodes aposc on ap.OrderStatusCodeId = aposc.id
			inner join OrderStatusCodes oosc on o.orderstatuscodeid = oosc.id
		where
			ap.UserId = @LoginId
			and ap.OrderStatusCodeId = 'CA'
			and aposc.level = oosc.level
			and ap.Completed = 0
	        and o.DateLastAction > @Cutoff
	) veditaccess
	where accessuserid is not null

	RETURN
END