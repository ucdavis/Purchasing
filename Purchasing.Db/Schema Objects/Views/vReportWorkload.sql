CREATE VIEW [dbo].[vReportWorkload]
	AS 

select row_number() over (order by ReportingWorkgroupId) id, ReportingWorkgroupId, ReportWorkgroup, userid, users.FirstName + ' ' + users.LastName UserFullName
	, WorkgroupName, WorkgroupOrg
	, sum(Approved) Approved, sum(Edited) Edited
from (

	select distinct w.id ReportingWorkgroupId, w.name ReportWorkgroup, vaw.workgroupid, vaw.primaryorganizationid WorkgroupOrg, vaw.name WorkgroupName
		, o.id, ot.userid, ot.orderstatuscodeid
		, case  when ot.[description] like '%approve%' then 1
				when ot.[description] like '%created%' then 1
				else 0
			end Approved
		, case when ot.[description] like '%edit%' then 1
				when ot.[description] like '%rerouted%' then 1
				else 0
			end Edited
	from workgroups w
		inner join workgroupsxorganizations wo on w.id = wo.WorkgroupId
		inner join vadminworkgroups vaw on wo.organizationid = vaw.rollupparentid
		inner join orders o on o.workgroupid = vaw.workgroupid
		inner join ordertracking ot on ot.orderid = o.id
		inner join workgrouppermissions wp on w.id = wp.workgroupid and wp.userid = ot.userid
	where administrative = 1

	union

	select w.id ReportingWorkgroupId, w.name ReportingWorkgroup, w.id WorkgroupId, w.PrimaryOrganizationId WorkgroupOrg, w.Name WorkgroupName
		, o.id, ot.userid, ot.orderstatuscodeid
		, case  when ot.[description] like '%approve%' then 1
				when ot.[description] like '%created%' then 1
				else 0
			end Approved
		, case when ot.[description] like '%edit%' then 1
				when ot.[description] like '%rerouted%' then 1
				else 0
			end Edited
	from workgroups w
		inner join orders o on o.workgroupid = w.id
		inner join ordertracking ot on ot.orderid = o.id
		inner join WorkgroupPermissions wp on w.id = wp.WorkgroupId and wp.userid = ot.userid
	where administrative = 0

) TrackingHistory
	inner join users on users.id = trackinghistory.userid
group by ReportingWorkgroupId, ReportWorkgroup, WorkgroupName, WorkgroupOrg, UserId, firstname, lastname