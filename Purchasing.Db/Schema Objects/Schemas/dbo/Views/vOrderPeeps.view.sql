CREATE VIEW dbo.vOrderPeeps
AS

select ROW_NUMBER() over (order by orderid) id, orderid, workgroupid, orderstatuscodeid, userid, fullname, administrative, sharedorcluster, roleid
from (
	select orders.id orderid, orders.workgroupid, approvals.OrderStatusCodeId, wp.userid, users.firstname + ' ' + users.lastname fullname, 0 administrative, 0 sharedorcluster, wp.roleid
	from orders 
		inner join approvals on approvals.OrderId = orders.id
		inner join WorkgroupPermissions wp on orders.WorkgroupId = wp.workgroupid and approvals.OrderStatusCodeId = wp.roleid
		inner join users on wp.userid = users.id
	where approvals.UserId is null
	union
	select orders.id orderid, orders.WorkgroupId, approvals.OrderStatusCodeId, admins.userid, admins.fullname, 1 administrative, admins.SharedOrCluster, admins.roleid
	from orders
		inner join approvals on approvals.OrderId = orders.id
		inner join (
			select workgroups.id adminworkgroupid, workgroups.SharedOrCluster, descendants.rollupparentid, descendants.orgid, descendantwrkgrp.id descendantworkgroupid
				, users.id userid, users.firstname + ' ' + users.lastname fullname
				, perms.roleid
			from workgroups
				inner join WorkgroupsXOrganizations on workgroups.id = WorkgroupsXOrganizations.workgroupid
				inner join vorganizationdescendants descendants on workgroupsxorganizations.organizationid = descendants.rollupparentid
				inner join workgroupsxorganizations descendantorgs on descendantorgs.organizationid = descendants.orgid
				inner join workgroups descendantwrkgrp on descendantwrkgrp.id = descendantorgs.workgroupid and descendantwrkgrp.administrative = 0
				inner join workgrouppermissions perms on workgroups.id = perms.workgroupid
				inner join users on perms.userid = users.id
			where workgroups.administrative = 1
		) admins on orders.workgroupid = admins.descendantworkgroupid
) peeps
where orderstatuscodeid = roleid