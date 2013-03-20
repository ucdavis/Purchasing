/*
Permissions access for open closed only.  Everyone essentially becomes readonly.
Orders that are in the following status codes: ('CN', 'CP', 'OC', 'OD')

3/12/2013
*/

CREATE VIEW [dbo].[vClosedAccess]
	AS 

select ROW_NUMBER() over (order by orderid) id, *
from (
	select distinct vClosedAccess.orderid, vClosedAccess.accessuserid, [admin] isadmin, OrderStatusCodeId accesslevel
	from
	(
		select orderid, userid accessuserid, 0 [admin], ordertracking.OrderStatusCodeId
		from ordertracking
			inner join orders o on ordertracking.orderid = o.id
		where o.orderstatuscodeid in ('CN', 'CP', 'OC', 'OD')

		union

		-- reviewer role
		select o.id orderid, wp.userid accessuserid
			, cast (case when wp.isadmin = 1 and wp.isfullfeatured = 0 then 1 else 0 end as bit) [admin]
			, wp.RoleId orderstatuscodeid
		from workgrouppermissions wp
			inner join orders o on o.WorkgroupId = wp.WorkgroupId
			inner join workgroups w on wp.workgroupid = w.id
		where wp.roleid = 'RV'
		  and o.orderstatuscodeid in ('CN', 'CP', 'OC', 'OD')

	) vClosedAccess
) cAccess


	