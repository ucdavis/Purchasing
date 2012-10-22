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