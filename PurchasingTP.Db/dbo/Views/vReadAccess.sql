CREATE VIEW [dbo].[vReadAccess]

	AS 

select ROW_NUMBER() over (order by orderid) id, access.orderid, access.UserId accessuserid, access.IsAway, OrderStatusCodeId accesslevel, [admin]
from
(
-- acted on order
select distinct orderid, userid, users.IsAway, OrderStatusCodeId, 0 [admin]
from ordertracking
	inner join Users on users.Id = ordertracking.userid

union

-- reviewer role
select distinct o.id orderid, wp.userid, users.IsAway, wp.RoleId
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