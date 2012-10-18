CREATE VIEW [dbo].[vWorkgroupRoles]
	AS 

select row_number() over (order by workgroupid) id, workgroupid, accessuserid, roleid, isadmin
from
(
select wk.id workgroupid, users.id accessuserid, roles.id roleid
	, cast (
		case when perms.IsAdmin = 1 and perms.IsFullFeatured = 0 then 1
		else 0 end
		as bit
	) as isadmin
from workgroups wk
	inner join workgrouppermissions perms on wk.id = perms.workgroupid
	inner join users on perms.userid = users.id
	inner join roles on perms.roleid = roles.id
where wk.Administrative = 0
) workgrouproles