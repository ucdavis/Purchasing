/*

	Description:

	This query is suppoedly suppoed to give a list of all sub-orgs that a given admin/parent that a workgroup/org might have access to.
	It also includes userid's to simplify querying and restricts orders that an admin should have access to

	Provides a list of "Pending" orders that an admin might have access to.  Readonly permissions should be derived from the ordertracking

	Updates:

		4/4/2012:	Added another query to it, so that it takes in the Shared Services or Cluster Admin workgroups.  Should give access to those special admin workgroups
					to differentiate from regular workgroups

*/

CREATE VIEW [dbo].[vAdminOrderAccess]
	
	AS 


select row_number() over (order by orderaccess.adminworkgroupid) id, orderaccess.adminworkgroupid, orderaccess.SharedOrCluster, orderaccess.OrgId
	, orderaccess.RollupParentId, orderaccess.descendantworkgroupid, orderaccess.orderid
	, orderaccess.userid accessuserid, orderaccess.IsAway
	, orderaccess.roleid
	, orderaccess.orderstatuscode
	, orderaccess.iscomplete
	, orderaccess.ispending
from
(
	/*
		Standard Administrative Workgroup Access
	*/
	select admins.*, orders.id orderid
		, cast(case when osc.Level = admins.rolelevel then 1 else 0 end as bit) ispending
		, osc.id orderstatuscode, osc.IsComplete
	from orders
	inner join (
		select workgroups.id adminworkgroupid, workgroups.SharedOrCluster, descendants.rollupparentid, descendants.orgid, descendantwrkgrp.id descendantworkgroupid
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

	union

	/*
		Shared Or Cluster Administrative Workgroup Access
	*/
	select admins.*, orders.id orderid
		, cast(case when os.Level = admins.rolelevel then 1 else 0 end as bit) ispending
		, os.id orderstatuscode, os.IsComplete
	from orders
	inner join orderstatuscodes os on orders.orderstatuscodeid = os.id
	inner join vapprovals va on va.orderid = orders.id and os.level = va.level and va.isworkgroup = 1
	inner join
	(
		select workgroups.id adminworkgroupid, workgroups.SharedOrCluster, descendants.rollupparentid, descendants.orgid, descendantwrkgrp.id descendantworkgroupid
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
		  and workgroups.SharedOrCluster = 1
	) admins on orders.workgroupid = admins.descendantworkgroupid
	where os.Level = admins.rolelevel

) orderaccess


