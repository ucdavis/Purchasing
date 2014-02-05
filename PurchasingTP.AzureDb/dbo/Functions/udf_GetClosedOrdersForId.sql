CREATE FUNCTION udf_GetClosedOrdersForId 
(
	@LoginId varchar(50) 
)
RETURNS 
@CLosedOrders TABLE 
(
	id int, 
	orderid int, 
	accessuserid varchar(10), 
	isadmin bit, 
	accesslevel char(2) 
)
AS
BEGIN
	INSERT INTO @CLosedOrders
	select ROW_NUMBER() over (order by orderid) id, orderid, accessuserid, isadmin, accesslevel
from (
	select  vClosedAccess.orderid, vClosedAccess.accessuserid, [admin] isadmin, OrderStatusCodeId accesslevel
	from
	(
		select orderid, userid accessuserid, 0 [admin], ordertracking.OrderStatusCodeId
		from ordertracking
			inner join orders o on ordertracking.orderid = o.id
		where o.orderstatuscodeid in ('CN', 'CP', 'OC', 'OD') and userid = @LoginId

		union

		-- reviewer role
		select o.id orderid, wp.userid accessuserid
			, cast (case when wp.isadmin = 1 and wp.isfullfeatured = 0 then 1 else 0 end as bit) [admin]
			, wp.RoleId orderstatuscodeid
		from workgrouppermissions wp
			inner join orders o on o.WorkgroupId = wp.WorkgroupId
			inner join workgroups w on wp.workgroupid = w.id
		where wp.roleid = 'RV'
		  and o.orderstatuscodeid in ('CN', 'CP', 'OC', 'OD') AND wp.userid = @LoginId

	) vClosedAccess
) cAccess

RETURN
END