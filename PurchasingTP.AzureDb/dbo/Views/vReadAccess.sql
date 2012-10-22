/*

	Description:

	Determines read access based on order tracking or reviewer role, regardless of order status.

*/

CREATE VIEW [dbo].[vReadAccess]

	AS 

select distinct access.orderid, access.UserId accessuserid,	[admin] isadmin, OrderStatusCodeId accesslevel
from
(

-- acted on order
select orderid, userid, OrderStatusCodeId, 0 [admin]
from ordertracking

union

-- reviewer role
select o.id orderid, wp.userid, wp.RoleId
    , cast (case when wp.isadmin = 1 and wp.isfullfeatured = 0 then 1 else 0 end as bit) [admin]
from workgrouppermissions wp
    inner join orders o on o.WorkgroupId = wp.WorkgroupId
    inner join workgroups w on wp.workgroupid = w.id and w.IsActive = 1
where wp.roleid = 'RV'

) access