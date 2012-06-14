/*

	Description:

	Provides a list of workgroups, and the possible orgs that it can roll up to.  To be used i guess for associating admin workgroups to their descendant workgroups.

*/

CREATE VIEW [dbo].[vAdminWorkgroups]

	AS 

select row_number() over (order by workgroups.id) id, workgroups.id workgroupid, workgroups.name, workgroups.primaryorganizationid
	, od.rollupparentid
from workgroups
	inner join workgroupsxorganizations wo on workgroups.id = wo.workgroupid
	inner join vorganizationdescendants od on wo.organizationid = od.orgid