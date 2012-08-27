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
		select orderid, accessuserid, 1 readaccess, 0 editaccess, 0 isadmin, isaway, accesslevel
		from vEditAccess	
	)
	)
) access