CREATE VIEW [dbo].[vWorkgroupRoles]
	AS 

select row_number() over (order by workgroupid) id, workgroupid, accessuserid, roleid, isadmin
from
(
select descendantworkgroupid workgroupid, userid accessuserid, roleid, 1 isadmin
from [vAdminWorkgroupRoles]

union

select wk.id workgroupid, users.id accessuserid, roles.id roleid, 0 isadmin
from workgroups wk
	inner join workgrouppermissions perms on wk.id = perms.workgroupid
	inner join users on perms.userid = users.id
	inner join roles on perms.roleid = roles.id
where wk.Administrative = 0
) workgrouproles