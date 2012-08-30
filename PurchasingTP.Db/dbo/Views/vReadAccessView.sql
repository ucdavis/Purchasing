CREATE VIEW [dbo].[vReadAccessView]

	AS 

select distinct access.orderid, access.UserId accessuserid,	cast(1 as bit) readaccess, cast(0 as bit) editaccess, [admin] isadmin, OrderStatusCodeId accesslevel
from
(

-- acted on order
select orderid, userid, users.IsAway, OrderStatusCodeId, 0 [admin]
from ordertracking
	inner join Users on users.Id = ordertracking.userid

union

-- reviewer role
select o.id orderid, wp.userid, users.IsAway, wp.RoleId
	, cast(case when wp.isadmin = 1 and wp.isfullfeatured = 0 then 1
		else 0
		end as bit) [admin]
from workgrouppermissions wp
	inner join Users on users.id = wp.UserId
	inner join Workgroups wk on wk.id = wp.WorkgroupId
	inner join orders o on o.WorkgroupId = wp.WorkgroupId
where wp.roleid = 'RV' 
  and wk.Administrative = 0
  and wk.IsActive = 1

) access