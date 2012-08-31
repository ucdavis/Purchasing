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

select ROW_NUMBER() over ( order by orderid ) id, * 
from (select distinct o.id orderid
	, case when ap.userid is null then wp.userid
			when wp.isadmin = 1 and wp.isfullfeatured = 0 then wp.userid
			when ap.userid is not null and ouser.isaway = 1 then wp.userid
			else ap.userid
			end accessuserid
	, cast (case when wp.isadmin = 1 and wp.isfullfeatured = 0 then 1 else 0 end as bit) isadmin
	, ap.orderstatuscodeid accesslevel
from orders o
	inner join orderstatuscodes osc on o.orderstatuscodeid = osc.id
	left outer join approvals ap on o.id = ap.orderid
	inner join orderstatuscodes aposc on ap.OrderStatusCodeId = aposc.id
	left outer join workgrouppermissions wp on o.workgroupid = wp.workgroupid and ap.orderstatuscodeid = wp.roleid
	left outer join users ouser on ouser.id = ap.userid
where ap.Completed = 0
	and osc.iscomplete = 0
	and aposc.level = osc.Level
union
select ap.OrderId, ap.SecondaryUserId accessuserid, cast(1 as bit) readaccess, cast(1 as bit) editaccess, cast(0 as bit) isadmin, ap.OrderStatusCodeId
from approvals ap
	inner join orders o on ap.OrderId = o.id
	inner join OrderStatusCodes aposc on ap.OrderStatusCodeId = aposc.id
	inner join OrderStatusCodes oosc on o.orderstatuscodeid = oosc.id
where ap.OrderStatusCodeId = 'CA'
	and ap.SecondaryUserId is not null
	and aposc.level = oosc.level
	and ap.Completed = 0
) veditaccess
where accessuserid is not null