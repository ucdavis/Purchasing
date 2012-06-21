/*

	Description:

	This query returns workgroup roles, based on administrative privileges (regardless of sharedorcluster option).

*/

CREATE VIEW [dbo].[vAdminWorkgroupRoles]
	AS 

select ROW_NUMBER() over (order by workgroups.id) id, workgroups.id adminworkgroupid, workgroups.SharedOrCluster, descendants.rollupparentid, descendants.orgid, descendantwrkgrp.id descendantworkgroupid
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
  and workgroups.IsActive = 1