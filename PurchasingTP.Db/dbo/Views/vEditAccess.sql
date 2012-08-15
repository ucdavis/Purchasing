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

select ROW_NUMBER() over ( order by userid ) id, access.orderid, access.UserId accessuserid, access.IsAway, OrderStatusCodeId accesslevel, [admin]
from
(

-- primary user specified, and not away (regular or CA)
select orders.id orderid, approvals.userid, users.isaway, approvals.OrderStatusCodeId, 0 [admin]
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

union

-- capture the conditional approvals, secondary user
select orders.id orderid, approvals.secondaryuserid, users.isaway, approvals.OrderStatusCodeId, 0 [admin]
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

union

-- workgroup permissions
select orders.id orderid, workgrouppermissions.userid, users.isaway, approvals.OrderStatusCodeId
	, cast(case when workgrouppermissions.isadmin = 1 and workgrouppermissions.isfullfeatured = 0 then 1
		   else 0
		   end as bit) [admin]
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
  and workgroups.IsActive = 1

union

-- capture the away approvals that are not conditioanl approvals
select orders.id orderid, workgrouppermissions.userid, users.isaway, approvals.OrderStatusCodeId
	, cast(case when workgrouppermissions.isadmin = 1 and workgrouppermissions.isfullfeatured = 0 then 1
		else 0
		end as bit) [admin]
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
  and workgroups.IsActive = 1
) access