/*

	Description:

	This query is suppoedly suppoed to give a list of all sub-orgs that a given admin/parent that a workgroup/org might have access to.
	It also includes userid's to simplify querying and restricts orders that an admin should have access to

	Provides a list of "Pending" orders that an admin might have access to.  Readonly permissions should be derived from the ordertracking

*/

CREATE VIEW [dbo].[vAdminOrderAccess]
	
	AS 

select row_number() over (order by orderaccess.adminworkgroupid) id, orderaccess.adminworkgroupid, orderaccess.OrgId
	, orderaccess.RollupParentId, orderaccess.descendantworkgroupid, orderaccess.orderid
	, orderaccess.userid accessuserid, orderaccess.IsAway
	, orderaccess.roleid
	, orderaccess.orderstatuscode
	, orderaccess.iscomplete
	, orderaccess.ispending
from
(
	select admins.*, orders.id orderid
		, cast(case when osc.Level = admins.rolelevel then 1 else 0 end as bit) ispending
		, osc.id orderstatuscode, osc.IsComplete
	from orders
	inner join (
		select workgroups.id adminworkgroupid, descendants.rollupparentid, descendants.orgid, descendantwrkgrp.id descendantworkgroupid
			, users.id userid, users.firstname + ' ' + users.lastname fullname, users.IsAway
			, roles.level rolelevel, roles.id roleid
			
		from workgroups
			inner join WorkgroupsXOrganizations on workgroups.id = WorkgroupsXOrganizations.workgroupid
			inner join vorganizationdescendants descendants on workgroupsxorganizations.organizationid = descendants.rollupparentid
			inner join workgroupsxorganizations descendantorgs on descendantorgs.organizationid = descendants.orgid
			inner join workgroups descendantwrkgrp on descendantwrkgrp.id = descendantorgs.workgroupid and descendantwrkgrp.administrative = 0
			inner join workgrouppermissions perms on workgroups.id = perms.workgroupid
			inner join users on perms.userid = users.id
			inner join roles on perms.roleid = roles.id
		where workgroups.administrative = 1
		  and workgroups.SharedOrCluster = 0
	) admins on orders.workgroupid = admins.descendantworkgroupid
	inner join OrderStatusCodes osc on orders.OrderStatusCodeId = osc.Id
) orderaccess