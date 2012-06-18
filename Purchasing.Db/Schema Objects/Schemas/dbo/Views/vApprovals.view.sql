/*

	Description:

	Logic for determining when an approval goes out to the workgroup permissions.

	Conditions for workgroup permissions:
		* Approval is not conditional approval
		* Either: Primary user is not assigned or primary user is away

*/

CREATE VIEW [dbo].[vApprovals]
	AS 

select distinct approvals.id, completed, orderstatuscodeid, orderid
	, os.level
	, pusers.id primaryuserid, pusers.isaway primaryaway
	, susers.id secondaryuserid, susers.isaway secondaryaway
	, cast (case
		when orderstatuscodeid <> 'CA' and pusers.id is null then 1
		when orderstatuscodeid <> 'CA' and pusers.id is not null and pusers.isaway = 1 then 1
		else 0
	  end as bit) IsWorkgroup	
from approvals
inner join orderstatuscodes os on os.id = approvals.orderstatuscodeid
left outer join users pusers on approvals.userid = pusers.id
left outer join users susers on approvals.secondaryuserid = susers.id