CREATE VIEW [dbo].[vAdminWorkgroups]

	AS 

select row_number() over (order by workgroups.id) id, workgroups.id workgroupid, workgroups.name, workgroups.primaryorganizationid
	, od.rollupparentid
from workgroups
	inner join workgroupsxorganizations wo on workgroups.id = wo.workgroupid
	inner join vorganizationdescendants od on wo.organizationid = od.orgid