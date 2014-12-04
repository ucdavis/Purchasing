
-- =============================================
-- Author:		Ken Taylor
-- Create date: February 10, 2014
-- Description:	Given a Kerberos/UCD Login ID, return all approvals specific to the user provided.
--	Notes: 
--		Replaces vApprovals.
--		Logic for determining when an approval goes out to the workgroup permissions.
--
/*
	Conditions for workgroup permissions:
		* Approval is not conditional approval
		* Either: Primary user is not assigned or primary user is away
*/
-- Usage:
/*
	select * from udf_GetApprovalsForLogin('bazemore')
*/
-- Modifications:
-- =============================================
CREATE FUNCTION [dbo].udf_GetApprovalsForLogin
(
	@LoginId varchar(50) 
)
RETURNS 
	@Approvals TABLE 
	(
	   [id] int
      ,[completed] bit
      ,[orderstatuscodeid] char(2)
      ,[orderid] int
      ,[level] int
      ,[primaryuserid] varchar(10)
      ,[primaryaway] int
      ,[secondaryuserid] int
      ,[secondaryaway] bit
      ,[IsWorkgroup] bit
	)
AS
BEGIN
	INSERT INTO @Approvals
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
	WHERE approvals.userid = @LoginId OR approvals.secondaryuserid = @LoginId
	
	RETURN 
END