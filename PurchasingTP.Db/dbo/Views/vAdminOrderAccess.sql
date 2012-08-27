CREATE VIEW [dbo].[vAdminOrderAccess]
	
	AS 

--
	
/*
Administrative access to orders
*/
select distinct row_number() over (order by perms.parentworkgroupid) id
	, perms.ParentWorkgroupId adminworkgroupid, perms.IsFullFeatured
	, perms.WorkgroupId descendantworkgroupid
	, orders.id orderid
	, u.id accessuserid, u.isaway
	, roles.id roleid
	, osc.id orderstatuscode
	, osc.iscomplete
	, cast(case when osc.Level = roles.Level then 1 else 0 end as bit) ispending
from WorkgroupPermissions perms
	inner join Roles on perms.roleid = roles.id and roles.IsAdmin = 0
	inner join orders on orders.WorkgroupId = perms.WorkgroupId
	inner join OrderStatusCodes osc on orders.OrderStatusCodeId = osc.id
	inner join users u on perms.userid = u.id
where perms.IsAdmin = 1 
	and perms.IsFullFeatured = 0
	and ((roles.id = osc.id and roles.id <> 'RV') or (roles.id = 'RV'))


--select row_number() over (order by orderaccess.adminworkgroupid) id, orderaccess.adminworkgroupid
--	, orderaccess.IsFullFeatured, orderaccess.OrgId
--	, orderaccess.RollupParentId, orderaccess.descendantworkgroupid, orderaccess.orderid
--	, orderaccess.userid accessuserid, orderaccess.IsAway
--	, orderaccess.roleid
--	, orderaccess.orderstatuscode
--	, orderaccess.iscomplete
--	, orderaccess.ispending
--from
--(
--	/*
--		Standard Administrative Workgroup Access
--	*/
--	select admins.*, orders.id orderid
--		, cast(case when osc.Level = admins.rolelevel then 1 else 0 end as bit) ispending
--		, osc.id orderstatuscode, osc.IsComplete
--	from orders
--	inner join (
--		select adminworkgroupid, IsFullFeatured, rollupparentid, orgid, descendantworkgroupid, userid, fullname, IsAway, rolelevel, roleid	 
--		from vAdminWorkgroupRoles where IsFullFeatured = 0
--	) admins on orders.workgroupid = admins.descendantworkgroupid
--	inner join OrderStatusCodes osc on orders.OrderStatusCodeId = osc.Id

--	union

--	/*
--		Shared Or Cluster Administrative Workgroup Access
--	*/
--	select admins.*, orders.id orderid
--		, cast(case when os.Level = admins.rolelevel then 1 else 0 end as bit) ispending
--		, os.id orderstatuscode, os.IsComplete
--	from orders
--	inner join orderstatuscodes os on orders.orderstatuscodeid = os.id
--	inner join vapprovals va on va.orderid = orders.id and os.level = va.level and va.isworkgroup = 1
--	inner join
--	(
--		select adminworkgroupid, IsFullFeatured, rollupparentid, orgid, descendantworkgroupid, userid, fullname, IsAway, rolelevel, roleid	 
--		from vAdminWorkgroupRoles where IsFullFeatured = 1
--	) admins on orders.workgroupid = admins.descendantworkgroupid
--	where os.Level = admins.rolelevel

--) orderaccess