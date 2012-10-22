CREATE VIEW [dbo].[vWorkgroupAdmins]
	as

select distinct ROW_NUMBER() over (order by wk.id) id
	, wk.id WorkgroupId, wk.name WorkgroupName, wk.PrimaryOrganizationId, users.id userid, users.firstname, users.lastname, users.email
from workgroups wk
	inner join vOrganizationDescendants vod on wk.PrimaryOrganizationId = vod.OrgId
	inner join UsersxOrganizations uo on vod.RollupParentId = uo.OrganizationId
	inner join users on uo.UserId = users.id