CREATE VIEW [dbo].[vRelatedWorkgroups]
	AS 
	select row_number() over (order by wk.id) id
       , wk.id workgroupid, wk.name, wk.primaryorganizationid
       , od.rollupparentid
       , adminwks.*
	   

from workgroups wk
       inner join workgroupsxorganizations wo on wk.id = wo.workgroupid
       inner join vorganizationdescendants od on wo.organizationid = od.orgid
       inner join (
              select awk.id adminworkgroupid, awo.OrganizationId adminorgid
              from workgroups awk
                     inner join WorkgroupsXOrganizations awo on awk.id = awo.WorkgroupId
              where awk.Administrative = 1
       ) adminwks on adminwks.adminorgid = od.orgid
	   WHERE Administrative = 0

