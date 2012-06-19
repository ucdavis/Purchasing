CREATE VIEW [dbo].[vAccess]
	AS 

select ROW_NUMBER() over (order by orderid) id, access.orderid, access.accessuserid, access.readaccess, access.editaccess, access.isadmin, access.IsAway, accesslevel
from
	(
	-- get edit access
	select orderid, accessuserid, 1 readaccess, 1 editaccess, 0 isadmin, isaway, accesslevel
	from vEditAccess
	union
	(
	select orderid, accessuserid, 1 readaccess, ispending editaccess
		, CAST (
			case when SharedOrCluster = 1 then 0
			else 1
			end as bit ) isadmin, isaway, roleid accesslevel
	from vAdminOrderAccess
	except
	select orderid, accessuserid, 1 readaccess, 1 editaccess, 0 isadmin, isaway, accesslevel
	from vEditAccess
	)
	union
	(
	-- get user with read access not with edit access
	select orderid, accessuserid, 1 readaccess, 0 editaccess, 0 isadmin, isaway, accesslevel
	from vReadAccess
	except 
	(
		select orderid, accessuserid, 0 readaccess, ispending editaccess, 1 isadmin, 0, roleid
		from vAdminOrderAccess
		union
		select orderid, accessuserid, 1 readaccess, 0 editaccess, 0 isadmin, isaway, accesslevel
		from vEditAccess	
	)
	)
) access