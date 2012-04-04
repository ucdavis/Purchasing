/*

	Description:

	Returns a list of users who have edit access to the associated order

	Ways to have permissions:
	
		*ALL APPROVALS EXCEPT CA
		*Specified as primary user and not away
		*Primary user is away, go to workgroup permissions
		*No user defined, go to workgroup permissions
		
		*CA APPROVALS
		*Specified as primary user
		*Specified as secondary user
		

*/

CREATE VIEW [dbo].[vEditAccess]

	AS 

select ROW_NUMBER() over ( order by userid ) id, access.orderid, access.UserId accessuserid, access.IsAway
from
(
-- primary user specified, and not away
select orders.id orderid, approvals.userid, users.isaway
from orders
	-- order's current status
	inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
	-- approvals at the same level as the order status and not completed
	inner join approvals on approvals.orderid = orders.id and approvals.orderstatuscodeid = os.id and approvals.completed = 0
	inner join users on users.id = approvals.userid
where approvals.userid is not null
  and os.IsComplete = 0
union
-- secondary user specified, away status doesn't matter
select orders.id orderid, approvals.secondaryuserid, users.isaway
from orders
	-- order's current status
	inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
	-- approvals at the same level as the order status and not completed
	inner join approvals on approvals.orderid = orders.id and approvals.orderstatuscodeid = os.id and approvals.completed = 0
	inner join users on users.id = approvals.secondaryuserid
where secondaryuserid is not null
  and os.IsComplete = 0
union
-- workgroup permissions
select orders.id orderid, workgrouppermissions.userid, users.isaway
from orders
	-- order's current status
	inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
	-- approvals at the same level as the order status and not completed
	inner join approvals on approvals.orderid = orders.id and approvals.orderstatuscodeid = os.id and approvals.completed = 0
	-- join with the workgroup
	inner join workgroups on orders.workgroupid = workgroups.id
	-- workgroup permissions
	inner join workgrouppermissions on workgroups.id = workgrouppermissions.workgroupid and orders.orderstatuscodeid = workgrouppermissions.roleid
	inner join users on users.id = WorkgroupPermissions.userid
where approvals.userid is null and approvals.secondaryuserid is null
  and os.IsComplete = 0
union
-- capture the away approvals that are not conditioanl approvals
select orders.id orderid, workgrouppermissions.userid, users.isaway
from orders
	-- order's current status
	inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
	-- approvals at the same level as the order status and not completed
	inner join approvals on approvals.orderid = orders.id and approvals.orderstatuscodeid = os.id and approvals.completed = 0
	-- join with the workgroup
	inner join workgroups on orders.workgroupid = workgroups.id
	-- workgroup permissions
	inner join workgrouppermissions on workgroups.id = workgrouppermissions.workgroupid and orders.orderstatuscodeid = workgrouppermissions.roleid
	-- join for the approval user to make sure they are away
	inner join users on users.id = approvals.userid
where approvals.userid is null and approvals.secondaryuserid is null
  and users.isaway = 1
  and approvals.orderstatuscodeid <> 'CA'
  and os.IsComplete = 0
union
-- capture the conditional approvals, primary user
select orders.id orderid, approvals.userid, users.isaway
from orders
	-- order's current status
	inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
	-- approvals at the same level as the order status and not completed
	inner join approvals on approvals.orderid = orders.id and approvals.completed = 0
	-- approval's status
	inner join orderstatuscodes aos on aos.id = approvals.orderstatuscodeid and aos.level = os.level
	inner join users on users.id = approvals.userid
where approvals.userid is not null
  and os.IsComplete = 0
  and approvals.orderstatuscodeid = 'CA'
union
-- capture the conditional approvals, secondary user
select orders.id orderid, approvals.secondaryuserid, users.isaway
from orders
	-- order's current status
	inner join orderstatuscodes os on os.id = orders.orderstatuscodeid
	-- approvals at the same level as the order status and not completed
	inner join approvals on approvals.orderid = orders.id and approvals.completed = 0
	-- approval's status
	inner join orderstatuscodes aos on aos.id = approvals.orderstatuscodeid and aos.level = os.level
	inner join users on users.id = approvals.secondaryuserid
where approvals.secondaryuserid is not null
  and os.IsComplete = 0
  and approvals.orderstatuscodeid = 'CA'
) access


/*
	ANLAI: The query below matches the above version as of 4/4/2012, but is slightly slower than the above version.
*/

/*
select ROW_NUMBER() over ( order by userid ) id, access.orderid, access.UserId accessuserid, access.IsAway
from
(
-- primary user
select orders.id orderid, va.primaryuserid userid, va.primaryaway isaway
from orders
	inner join orderstatuscodes os on orders.orderstatuscodeid = os.id
	inner join vapprovals va on va.level = os.level and orders.id = va.orderid and va.completed = 0
where os.iscomplete = 0
  and (va.primaryuserid is not null)

union

-- secondary user, only for conditional approvers
select orders.id orderid, va.secondaryuserid userid, va.secondaryaway isaway
from orders
	inner join orderstatuscodes os on orders.orderstatuscodeid = os.id
	inner join vapprovals va on va.level = os.level and orders.id = va.orderid and va.completed = 0
where os.iscomplete = 0
  and (va.secondaryuserid is not null)
  
union

-- workgroup permissions
select orders.id orderid, workgrouppermissions.userid, users.isaway
from orders
	inner join orderstatuscodes os on orders.orderstatuscodeid = os.id
	inner join vapprovals va on va.level = os.level and orders.id = va.orderid and va.completed = 0
	-- workgroup permissions
	inner join workgroups on orders.workgroupid = workgroups.id
	inner join workgrouppermissions on workgroups.id = workgrouppermissions.workgroupid and orders.orderstatuscodeid = workgrouppermissions.roleid
	inner join users on users.id = WorkgroupPermissions.userid
where os.iscomplete = 0
  and va.IsWorkgroup = 1
) access
order by orderid
*/