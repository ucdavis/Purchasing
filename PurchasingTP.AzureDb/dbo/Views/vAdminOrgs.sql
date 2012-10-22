/*

	Description:

	Shows a user's rights to departments based on departmental administrative roles and their assigned orgs (performs, descendant lookup as well)

*/

CREATE VIEW [dbo].[vAdminOrgs]
	AS 

select row_number() over (order by p.userid) id, p.userid accessuserid, od.orgid, od.name, od.immediateparentid, od.rollupparentid, od.isactive
from [permissions] p 
	inner join usersxorganizations uo on uo.userid = p.userid
	inner join vorganizationdescendants od on uo.organizationid = od.rollupparentid
where p.roleid = 'DA'