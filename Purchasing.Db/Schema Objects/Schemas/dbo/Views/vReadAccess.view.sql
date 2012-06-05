CREATE VIEW [dbo].[vReadAccess]

	AS 

select ROW_NUMBER() over (order by orderid) id, access.orderid, access.UserId accessuserid, access.IsAway, OrderStatusCodeId accesslevel
from
(
select distinct orderid, userid, users.IsAway, OrderStatusCodeId
from ordertracking
	inner join Users on users.Id = ordertracking.userid

union

select distinct o.id orderid, wp.userid, users.IsAway, wp.RoleId
from workgrouppermissions wp
	inner join Users on users.id = wp.UserId
	inner join Workgroups wk on wk.id = wp.WorkgroupId
	inner join orders o on o.WorkgroupId = wp.WorkgroupId
where wp.roleid = 'RV' 
  and wk.Administrative = 0

union

select distinct o.id orderid, awr.userid, users.IsAway, awr.RoleId
from vAdminWorkgroupRoles awr
	inner join users on awr.userid = users.id
	inner join orders o on awr.descendantworkgroupid = o.WorkgroupId
where awr.roleid = 'RV'
) access